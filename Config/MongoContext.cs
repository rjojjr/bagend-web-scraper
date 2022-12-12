using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace bagend_web_scraper.Config
{
    public class MongoContext
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoContext(
            IOptions<MongoDbConfig> databaseConfig)
        {

            MongoCredential credential = MongoCredential.CreateCredential(
                "admin", 
                databaseConfig.Value.Username,
                databaseConfig.Value.Password
            );
            var settings = new MongoClientSettings
            {
                Credential = credential,
                Server = new MongoServerAddress(
                    databaseConfig.Value.Host,
                    databaseConfig.Value.Port
                ),
                SocketTimeout = new TimeSpan(0, 3, 0),
                WaitQueueTimeout = new TimeSpan(0, 3, 0),
                ConnectTimeout = new TimeSpan(0, 3, 0)
            };
            var mongoClient = new MongoClient(settings);

            _mongoDatabase = mongoClient.GetDatabase(
                databaseConfig.Value.DatabaseName);
        }

        public IMongoDatabase GetMongoDatabase()
        {
            return _mongoDatabase;
        }
    }
}