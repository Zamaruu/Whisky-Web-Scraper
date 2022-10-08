using HtmlAgilityPack;
using WhiskyWebScraper.Data;
using WhiskyWebScraper.Data.Models;
using WhiskyWebScraper.Helper;

namespace WhiskyWebScraper
{
    public class WebSraper
    {
        private readonly string BaseUrl = $"https://www.whisky.de/flaschen-db/flaschen-suche/whisky/fdb/Flaschen/search.html?type=1505224767&tx_datamintsflaschendb_pi4%5BsearchCriteria%5D%5BsortingCombined%5D=bewertungsAnzahl_descending&tx_datamintsflaschendb_pi4%5BsearchCriteria%5D%5BspiritType%5D=1&&tx_datamintsflaschendb_pi4%5BresultsOnly%5D=1tx_datamintsflaschendb_pi4%5BcurPage%5D={1}";

        public WebSraper()
        {
        }

        // Scraper
        public async Task<List<string>> Start()
        {

            Console.WriteLine("Starting Scraping...");

            var response = await CallUrl(BaseUrl);

            Console.WriteLine("Scraping Finished.");

            if (response != null)
            {
                var links = ParseHtml(response);
                // await UploadLinks(links);

                if(links != null) return links;
            }

            Console.Error.WriteLine("Site could not be fetched!");
            return new List<string>();
        }



        // Worker Methods

        private async Task<string> CallUrl(string fullUrl)
        {

            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(fullUrl);
            return response;
        }



        private List<string>? ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            //var content = GetWhiskyContentSection(htmlDoc);
            //if (content == null) return null;

            var items = HtmlQueries.GetWhiskyItems(htmlDoc.DocumentNode);
            if (items == null) return null;

            List<string> links = new();
            
            foreach (var item in items)
            {
                var titleTag = item.Descendants("div")
                    .Where(node => node.GetClasses().Contains("title"))
                    .FirstOrDefault();
                
                var linkTag = titleTag.Descendants("a")
                    .FirstOrDefault();

                string hrefValue = linkTag.GetAttributeValue("href", string.Empty);
                links.Add($"https://www.whisky.de{hrefValue}");
                
                Console.WriteLine($"https://www.whisky.de{hrefValue}");
            }

            return links;
        }

        private HtmlNode? GetWhiskyContentSection(HtmlDocument htmlDoc)
        {
            var contentSection = HtmlQueries.GetWhiskyContentSection(htmlDoc);
            var resultContainer = HtmlQueries.GetWhiskySearchResultContainer(contentSection);
            var contentItems = HtmlQueries.GetWhiskyResultListContainer(resultContainer);

            return contentItems;
        }
    
        // private async Task UploadLinks(List<string> rawLinks)
        // {
        //     if (rawLinks == null)
        //     {
        //         Console.Error.WriteLine("Links could not be uploaded to mongodb");
        //         return;
        //     }

        //     Console.WriteLine("Start link updating...");

        //     var skippedLinks = 0;

        //     foreach (var link in rawLinks)
        //     {
        //         if (await mongoService.LinkExists(link))
        //         {
        //             skippedLinks++;
        //             continue;
        //         }

        //         var newLink = new WhiskyDetailLink(link);
        //         var result = await mongoService.SaveDocument(newLink, MongoCollections.WhiskyDeLinks);

        //         if (!result)
        //         {
        //             Console.Error.WriteLine($"{link} could not be uploaded to mongodb");
        //         }
        //     }

        //     Console.WriteLine($"Links successfully updated, {skippedLinks} Links were skipped.");
        // }
    }
}
