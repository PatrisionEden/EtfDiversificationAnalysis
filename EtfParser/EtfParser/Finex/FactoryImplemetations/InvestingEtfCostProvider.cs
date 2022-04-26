using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace EtfParser
{
    public class InvestingEtfCostProvider : IEtfCostProvider
    {
        private const string _priorityCurrency = "usd";
        private const string _url = @"https://ru.investing.com";
        private const string _searchPostfix = @"/search/?q=";
        private HttpClient _webClient;
        private ICurrencyConverter _currencyConverter;
        public InvestingEtfCostProvider(ICurrencyConverter currencyConverter)
        {
            _webClient = new HttpClient();
            _currencyConverter = currencyConverter;
        }
        public double? GetCostByTicker(string ticker)
        {
            Stream page = GetPage(_url + _searchPostfix + ticker);
            string route;
            try
            {
                route = ParseRouteFromHtmlPage(page);
            }
            catch(Exception e)
            {
                return null;
            }

            page = GetPage(_url + route);
            double cost = ParseCostFromHtmlPage(page);
            string currency = ParseCurrencyFromHtmlPage(page);

            if (currency != _priorityCurrency)
                cost = _currencyConverter.ConvertFromOneCurrencyToAnother(currency, _priorityCurrency, cost);

            return cost;
        }
        private Stream GetPage(string pageUrl)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, pageUrl);
            httpRequestMessage.Headers.Add("User-Agent", "User-Agent-Here");
            HttpResponseMessage response = _webClient.SendAsync(httpRequestMessage).Result;
            return response.Content.ReadAsStream();
        }
        private string ParseRouteFromHtmlPage(Stream pageContent)
        {
            IHtmlDocument document = new HtmlParser().ParseDocument(pageContent);
            IElement a = document.QuerySelectorAll("a.js-inner-all-results-quote-item")
                .Where(element => element.ClassName == "js-inner-all-results-quote-item row" && element != null).First();

            var atr = a.Attributes.Where(attr => attr.LocalName == "href").First().Value;

            return atr;
        }

        private double ParseCostFromHtmlPage(Stream content)
        {
            content.Position = 0;
            IHtmlDocument document = new HtmlParser().ParseDocument(content);
            string? div = document.QuerySelectorAll("#quotes_summary_current_data")
                .First()
                .FirstElementChild?
                .Children
                .Skip(1)
                .First()
                .FirstElementChild?
                .FirstElementChild?
                .InnerHtml;

            if (div == null)
                throw new Exception("Cannot parse cost from HTML page");
            div = div.Replace(".", "");
            return double.Parse(div);
        }

        private string ParseCurrencyFromHtmlPage(Stream content)
        {
            content.Position = 0;
            IHtmlDocument document = new HtmlParser().ParseDocument(content);

            string? div = document.QuerySelectorAll("#quotes_summary_current_data")
                .First()
                .FirstElementChild?
                .Children
                .Skip(1)
                .First()
                .LastElementChild?
                .Children
                .SkipLast(1)
                .Last()
                .InnerHtml;

            if (div == null)
                throw new Exception("Cannot parse currency from HTML page");

            return div.ToLower();
        }


    }
}
