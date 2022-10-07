using WhiskyWebScraper;

Console.WriteLine("Welcome to the Whisky Scraper!");

var scraper = new WebSraper();
await scraper.Start();