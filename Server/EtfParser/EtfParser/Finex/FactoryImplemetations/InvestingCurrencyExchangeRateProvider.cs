using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public class InvestingCurrencyExchangeRateProvider : ICurrensyExchangeRateProvider
    {
        private const string _url = @"https://ru.investing.com/currencies/";
        private HttpClient _webClient;
        public InvestingCurrencyExchangeRateProvider()
        {
            _webClient = new();
        }
        public double GetCurrencyRate(string firstCurrency, string secondCurrency)
        {
            string route = string.Format($"/{firstCurrency}-{secondCurrency}");
            Stream responseContent = GetPage(_url + route);

            double currencyRate;
            try
            {
                currencyRate = ParseRateFromPage(responseContent);
            }
            catch(Exception e)
            {
                route = string.Format($"/{secondCurrency}-{firstCurrency}");
                responseContent = GetPage(_url + route);
                currencyRate = ParseRateFromPage(responseContent);
                currencyRate = 1 / currencyRate;
            }

            return currencyRate;
        }
        private Stream GetPage(string pageUrl)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, pageUrl);
            httpRequestMessage.Headers.Add("User-Agent", "User-Agent-Here");
            HttpResponseMessage response = _webClient.SendAsync(httpRequestMessage).Result;
            //string s = response.Content.ReadAsStringAsync().Result;
            return response.Content.ReadAsStream();
        }
        private double ParseRateFromPage(Stream responseContent)
        {
            IHtmlDocument document = new HtmlParser().ParseDocument(responseContent);
            var temp = document.QuerySelectorAll("span.text-2xl")
                .First();
            string? rate = document.QuerySelectorAll("span.text-2xl")
                .Where(span => span.HasAttribute("data-test"))
                .Where(span => span.GetAttribute("data-test") == "instrument-price-last")
                .First()
                .InnerHtml;

            if (rate == null)
                throw new Exception("Cannot parse currency rate from HTML page");

            return double.Parse(rate.Replace(",", ""));
        }
    }
}
