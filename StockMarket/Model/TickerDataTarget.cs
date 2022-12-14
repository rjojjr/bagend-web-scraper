using System;
using bagend_web_scraper.StockMarket.Entity;

namespace bagend_web_scraper.StockMarket.Model
{
	public class TickerDataTarget
	{

        /// <summary>
        /// Automatically assigned UUID Primary key for DB document.
        /// </summary>
        /// <example>2ee4616c-187b-40ed-9db1-403e38ae34d7</example>
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

        public TickerDataTarget()
		{
		}

        public static TickerDataTarget FromEntity(TickerDataTargetEntity entity)
        {
            var tickerTarget = new TickerDataTarget();
            tickerTarget.Id = entity.Id;
            tickerTarget.Priority = entity.Priority;
            tickerTarget.TickerSymbol = entity.TickerSymbol;
            tickerTarget.CompanyName = entity.CompanyName;
            tickerTarget.BusinessSector = entity.BusinessSector;
            tickerTarget.IsStarted = entity.IsStarted;
            tickerTarget.IsCompleted = entity.IsCompleted;
            tickerTarget.IsActive = entity.IsActive;
            tickerTarget.LastDatapointTimeValue = entity.LastDatapointTimeValue;
            tickerTarget.TargetCreatedAt = entity.TargetCreatedAt;
            tickerTarget.LastUpdatedAt = entity.LastUpdatedAt;

            return tickerTarget;
        }

	}
}

