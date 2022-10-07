using MongoDB.Driver;
using WhiskyWebScraper.Data;
using WhiskyWebScraper.Data.Models;

namespace WhiskyWebScraper.MongoDB
{
    public class MongoService
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        private readonly string _connectionString = "mongodb://mongoadmin:McnncKOh2Ijt2R0lpB71yEZgT1W9snmE@194.163.182.236:27017";
        private readonly string _databaseName = "whisky-db";

        public MongoService()
        {
            _client = new MongoClient(_connectionString);
            _database = _client.GetDatabase(_databaseName);
        }

        private IMongoCollection<T> GetCollection<T>(MongoCollections collection)
        {
            switch (collection)
            {
                case MongoCollections.WhiskyDeLinks:
                    return _database.GetCollection<T>("whisky-de-links");
                default:
                    Console.Error.WriteLine("Collection could not be found!");
                    return null;
            }
        }

        public async Task<bool> SaveDocument<T>(T data, MongoCollections type)
        {
            var collection = GetCollection<T>(type);
            
            try
            {
                await collection.InsertOneAsync(data);
                return true;

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return true;
            }

        }

        public async Task<bool> LinkExists(string Link)
        {
            var collection = GetCollection<WhiskyDetailLink>(MongoCollections.WhiskyDeLinks);
            var item = await collection.Find(wdl => wdl.Link.Equals(Link)).FirstOrDefaultAsync();

            return item != null;
        }
    }
}
