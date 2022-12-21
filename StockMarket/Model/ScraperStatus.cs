using System;
namespace bagend_web_scraper.StockMarket.Model
{
	public class ScraperStatus
	{

		public long total { get; set; }
		public long completed { get; set; }
		public long timeElapsed { get; set; }
		public float averageRate { get; set; }
		public float remainingTime { get; set; }

		public ScraperStatus()
		{
		}

        public ScraperStatus(long total, long completed, long timeElapsed, float averageRate, float remainingTime)
        {
            this.total = total;
            this.completed = completed;
            this.timeElapsed = timeElapsed;
            this.averageRate = averageRate;
            this.remainingTime = remainingTime;
        }
    }
}

