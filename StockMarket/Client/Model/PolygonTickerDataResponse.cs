using System;
using System.Text.Json.Serialization;

namespace bagend_web_scraper.StockMarket.Client.Model
{
	public class PolygonTickerDataResponse
	{

		public class TickerResult
		{
            [JsonPropertyName("market")]
            public string Market { get; set; }

            [JsonPropertyName("locale")]
            public string Locale { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("ticker")]
            public string Ticker { get; set; }

        }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("results")]
        public IList<TickerResult> Results { get; set; } = new List<TickerResult>();


        public PolygonTickerDataResponse()
		{
		}
	}
}

