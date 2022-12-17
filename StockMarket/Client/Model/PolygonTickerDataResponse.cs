using System;
using System.Text.Json.Serialization;

namespace bagend_web_scraper.StockMarket.Client.Model
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

        [JsonExtensionData]
        private IDictionary<string, string> _additionalData;

        public TickerResult() { }

    }
    public class PolygonTickerDataResponse
	{

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("next_url")]
        public string NextUrl { get; set; }

        [JsonPropertyName("results")]
        public IList<TickerResult> Results { get; set; } = new List<TickerResult>();

        [JsonExtensionData]
        private IDictionary<string, string> _additionalData;


        public PolygonTickerDataResponse()
		{
		}
	}
}

