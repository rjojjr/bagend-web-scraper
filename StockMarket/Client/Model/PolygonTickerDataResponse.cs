using System;
namespace bagend_web_scraper.StockMarket.Client.Model
{
	public class PolygonTickerDataResponse
	{

		public class TickerResult
		{
			public string Market { get; set; }
			public string Locale { get; set; }
            public string Name { get; set; }
            public string Ticker { get; set; }

        }

		public int Count { get; set; }
		public IList<TickerResult> Results { get; set; } = new List<TickerResult>();


        public PolygonTickerDataResponse()
		{
		}
	}
}

