using MongoDB.Bson.Serialization.Attributes;

namespace bagend_web_scraper.StockMarket.Entity
{
    public class TickerDataTargetEntity
    {
        public TickerDataTargetEntity()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [BsonElement("_id")]
        [BsonId]
        public string Id { get; set; } = null!;

        public int Priority { get; set; } = 100;

        public string TickerSymbol { get; set; } = null!;

        public string CompanyName { get; set; } = null!;

        public string BusinessSector { get; set; } = null!;

        public bool IsStarted { get; set; } = false;

        public bool IsCompleted { get; set; } = false;

        public bool IsActive { get; set; } = false;

        public string LastDatapointTimeValue { get; set; } = null!;

        public DateTime TargetCreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public TickerDataTargetEntity withCurrentDateTimeAsCreationTimestamp()
        {
            TargetCreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;

            return this;
        }

        public TickerDataTargetEntity withCurrentDateTimeAsUpdateTimestamp()
        {
            LastUpdatedAt = DateTime.UtcNow;

            return this;
        }
    }
}