using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtfParser
{
    public class FinexEtfNamesWebProvider : IEtfNamesProvider
    {
        private const string _url = @"https://finexetf.com/product/";
        private HttpClient _webClient;
        public FinexEtfNamesWebProvider()
        {
            _webClient = new HttpClient();
        }
        public List<string> GetEtfNames()
        {
            HttpResponseMessage response = _webClient.GetAsync(_url).Result;
            Stream responseContent = response.Content.ReadAsStream();

            return ParseNamesFromPage(responseContent);
        }

        private List<string> ParseNamesFromPage(Stream responseContent)
        {
            List<string> names = new List<string>();

            IHtmlDocument htmlPage = new HtmlParser().ParseDocument(responseContent);
            IHtmlTableElement? tableWithEtfs = htmlPage.QuerySelector("table") as IHtmlTableElement;
            if (tableWithEtfs == null)
                throw new NullReferenceException("Cannot find table tag. Possibly, html is changed");
            IHtmlTableSectionElement tableBodyWithEtfs = tableWithEtfs.Bodies.First();

            foreach (var row in tableBodyWithEtfs.Rows)
            {
                names.Add(row.Cells.Last().TextContent.Split(" ").First());
            }

            return names;
        }
    }
}
