using System;
using System.Text.Json.Serialization;

namespace bagend_web_scraper.StockMarket.Client
{
	public class PolygonOpenCloseApiResponse
	{
        [JsonPropertyName("afterHours")]
        public decimal AfterHours { get; set; } = 0!;

        [JsonPropertyName("close")]
        public decimal Close { get; set; } = 0!;

        [JsonPropertyName("high")]
        public decimal High { get; set; } = 0!;

        [JsonPropertyName("low")]
        public decimal Low { get; set; } = 0!;

        [JsonPropertyName("open")]
        public decimal Open { get; set; } = 0!;

        [JsonPropertyName("volume")]
        public decimal Volume { get; set; } = 0!;

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = null!;

        [JsonPropertyName("from")]
        public string From { get; set; } = null!;

        public PolygonOpenCloseApiResponse()
		{
		}
	}
}

