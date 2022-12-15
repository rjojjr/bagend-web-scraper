using System.Threading;
using System.Threading.Tasks;
using bagend_web_scraper.Config;
using bagend_web_scraper.StockMarket.Entity;
using MongoDB.Driver;

namespace bagend_web_scraper.Repository
{
    public class TickerDataTargetEntityRepository
    {

        public const string CollectionName = "ticker_data_targets";

        private readonly IMongoCollection<TickerDataTargetEntity> _collection;

        public TickerDataTargetEntityRepository(
            MongoContext mongoContext)
        {
            _collection = mongoContext.GetMongoDatabase().GetCollection<TickerDataTargetEntity>(
                CollectionName);
        } 

        public async Task<List<TickerDataTargetEntity>> GetAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<List<TickerDataTargetEntity>> GetNextWorkAsync() =>
           await _collection.Find(_ => true).SortBy(x => x.Priority).ThenBy(x => x.LastUpdatedAt).ToListAsync();

        public async Task<List<string>> GetTickersAsync() =>
           await _collection.Find(_ => true).SortBy(x => x.Priority).ThenBy(x => x.LastUpdatedAt).Project(x => x.TickerSymbol).ToListAsync();

        public async Task<TickerDataTargetEntity?> GetAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(TickerDataTargetEntity newEvent) =>
        await _collection.InsertOneAsync(newEvent);

        public async Task<TickerDataTargetEntity?> GetByStockTickerAsync(string stockTicker) =>
        await _collection.Find(x => x.TickerSymbol == stockTicker).FirstOrDefaultAsync();


        public async Task UpdateAsync(string id, TickerDataTargetEntity updatedEvent) =>
            await _collection.ReplaceOneAsync(x => x.Id == id, updatedEvent);

        public async Task RemoveAsync(string id) =>
            await _collection.DeleteOneAsync(x => x.Id == id);
    }
}