using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public class InvestingShareDataProvider : IShareDataProvider
    {
        private ICurrencyConverter _currencyConverter;
        private const string _url = @"https://www.investing.com";
        private const string _searchRoute = @"/search/?q=";
        private const string _companyProfilePostfix = @"-company-profile";
        private HttpClient _webClient;
        public InvestingShareDataProvider(ICurrencyConverter currencyConverter)
        {
            _currencyConverter = currencyConverter;
            _webClient = new();
        }
        public InvestingShareDataProvider()
        {
            _currencyConverter = new CurrencyConverter(new InvestingCurrencyExchangeRateProvider());
            _webClient = new();
        }
        public IShareData GetShareDataByIsin(string isin)
        {
            Stream page = GetPage(_url + _searchRoute + isin);
            string route = ParseRouteFromHtmlPage(page, isin);

            string companyProfileRoute = "";
            if (route.Contains("?cid"))
            {
                int index = route.IndexOf("?cid");
                companyProfileRoute = _url + route.Insert(index, _companyProfilePostfix);
            }
            else if (route.StartsWith("/rates-bonds/"))
                throw new BondException("This is bond");
            else if (route.StartsWith("/etfs/"))
                throw new BondException("This is bond");
            else
                companyProfileRoute = _url + route + _companyProfilePostfix;
            page = GetPage(companyProfileRoute);
            string shareName = ParseSharenameFromPage(page).Trim(); ;
            string ticker = shareName.Substring(shareName.IndexOf('(') + 1, shareName.Length - shareName.IndexOf('(') - 2);
            string country = ParseCountryFromPage(page);
            string Industry = ParseIndustryFromPage(page);
            string Sector = ParseSectorFromPage(page);
            //string parsedIsin = isin;
            double partInEtf = default(double);
            double price = ParsePriceFromPage(page);
            string currency = ParseCurrencyFromPage(page);

            if (currency != "usd")
                price = _currencyConverter.ConvertFromOneCurrencyToAnother(currency, "usd", price);

            IShareData shareData = new ShareData(shareName);
            shareData.Initialise(ticker, country, Sector, Industry, isin, partInEtf, price);

            return shareData;
        }
        private Stream GetPage(string pageUrl)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, pageUrl);
            httpRequestMessage.Headers.Add("User-Agent", "User-Agent-Here");
            HttpResponseMessage response = _webClient.SendAsync(httpRequestMessage).Result;
            string s = response.Content.ReadAsStringAsync().Result;
            return response.Content.ReadAsStream();
        }
        private string ParseRouteFromHtmlPage(Stream pageContent, string query)
        {
            pageContent.Position = 0;
            IHtmlDocument document = new HtmlParser().ParseDocument(pageContent);
            IElement div = document.QuerySelectorAll("div.searchSection.allSection").First();
            if (div.Children.Count() == 1)
                throw new NoSearchResultException("Cannot find any " + query);
            IElement a = document.QuerySelectorAll("a.js-inner-all-results-quote-item")
                .Where(element => element.ClassName == "js-inner-all-results-quote-item row" && element != null).First();

            var atr = a.Attributes.Where(attr => attr.LocalName == "href").First().Value;

            return atr;
        }
        private string ParseSharenameFromPage(Stream pageContent)
        {
            pageContent.Position = 0;
            IHtmlDocument document = new HtmlParser().ParseDocument(pageContent);
            string? h1Content = document.QuerySelectorAll("div.instrumentHead").FirstOrDefault()?.FirstElementChild?.InnerHtml;

            if (h1Content == null)
            {
                pageContent.Position = 0;
                h1Content = document.QuerySelectorAll("h1").FirstOrDefault()?.TextContent;
            }

            if (h1Content == null)
                throw new NullReferenceException("Cannot parse shareName from page");

            return h1Content;
        }
        private string ParseCountryFromPage(Stream pageContent)
        {
            pageContent.Position = 0;
            IHtmlDocument document = new HtmlParser().ParseDocument(pageContent);
            string? spanContent = document.QuerySelectorAll("div.companyAddress").First()?
                .Children?
                .Skip(2).First()?
                .FirstElementChild?
                .LastElementChild?
                .TextContent;

            if (spanContent == null)
                throw new NullReferenceException("Cannot parse country from page");

            return spanContent;
        }
        private string ParseIndustryFromPage(Stream pageContent)
        {
            pageContent.Position = 0;
            IHtmlDocument document = new HtmlParser().ParseDocument(pageContent);
            string? aContent = document.QuerySelectorAll("div.companyProfileHeader").First()?
                .FirstElementChild?
                .LastElementChild?
                .TextContent;

            if (aContent == null)
                throw new NullReferenceException("Cannot parse country from page");

            return aContent;
        }
        private string ParseSectorFromPage(Stream pageContent)
        {
            pageContent.Position = 0;
            IHtmlDocument document = new HtmlParser().ParseDocument(pageContent);
            string? aContent = document.QuerySelectorAll("div.companyProfileHeader").First()?
                .Children?
                .Skip(1)
                .First()
                .LastElementChild?
                .TextContent;

            if (aContent == null)
                throw new NullReferenceException("Cannot parse country from page");

            return aContent;
        }
        private double ParsePriceFromPage(Stream pageContent)
        {
            pageContent.Position = 0;
            IHtmlDocument document = new HtmlParser().ParseDocument(pageContent);
            string? spanContent = document.QuerySelectorAll("#last_last").First()?
                .TextContent;

            if (spanContent == null)
                throw new NullReferenceException("Cannot parse country from page");

            return double.Parse(spanContent, CultureInfo.InvariantCulture);
        }
        private string ParseCurrencyFromPage(Stream content)
        {
            //content.Position = 0;
            //string html = new StreamReader(content).ReadToEnd();

            content.Position = 0;
            IHtmlDocument document = new HtmlParser().ParseDocument(content);

            string? div = document.QuerySelectorAll("#quotes_summary_current_data")
                .First()
                .FirstElementChild?
                .Children
                .Skip(1)
                .First()
                .Children
                .Skip(1)
                .First()
                .Children
                .SkipLast(1)
                .Last()
                .InnerHtml;

            if (div == null || div.Length != 3)
                throw new CurrencyParseException("Cannot parse currency from HTML page");

            return div.ToLower();
        }
    }
}
