using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace WhiskyWebScraper
{
    public class WebSraper
    {
        private readonly string BaseUrl = "https://www.whisky.de/flaschen-db/flaschen-suche.html";


        // Worker Methods

        private async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(fullUrl);
            return response;
        }

        // Scraper
        public async Task Start() {
            Console.WriteLine("Starting Scraping...");

            var response = await CallUrl(BaseUrl);

            Console.WriteLine("Scraping Finished.");

            if(response != null)
            {
                var elements = ParseHtml(response);
                Console.WriteLine(elements);
                return;
            } 
            
            Console.Error.WriteLine("Site could not be fetched!");
        }

        private List<string> ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var result = GetContentSection(htmlDoc);

            var items = result.Descendants("div")
                    .Where(node => node.GetClasses().Contains("item") && node.GetClasses().Contains("item-wrap"))
                    .ToList();


            if (items != null)
            {
                Console.WriteLine(items);
            }

            //foreach (var item in containers)
            //{
            //    Console.WriteLine(item.Name);
            //    foreach (var nodeClass in item.GetClasses())
            //    {
            //        Console.WriteLine(nodeClass);
            //    }
            //    Console.WriteLine("------------------");
            //}

            List<string> nodes = new List<string>();

            //foreach (var div in containers)
            //{
            //    if (div.FirstChild.Attributes.Count > 0) nodes.Add("https://en.wikipedia.org/" + div.FirstChild.Attributes[0].Value);
            //}

            return nodes;
        }

        private HtmlNode? GetContentSection(HtmlDocument htmlDoc)
        {
            var content = htmlDoc.DocumentNode.Descendants("div")
                .Where(node => node.GetClasses().Contains("content") && node.GetClasses().Contains("main"))
                .FirstOrDefault();

            if(content != null)
            {
                var resultContainer = content.Descendants("div")
                    .Where(node => node.GetClasses().Contains("search-result-container"))
                    .FirstOrDefault();

                if(resultContainer != null)
                {
                    var resultList = content.Descendants("div")
                        .Where(node => node.GetClasses().Contains("resultlist") && node.GetClasses().Contains("flaschen"))
                        .FirstOrDefault();

                    return resultList;
                }
                return null;
            }
            return null;
        }
    }
}
