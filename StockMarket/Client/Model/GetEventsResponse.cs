using System;
using System.Text.Json.Serialization;

namespace bagend_web_scraper.StockMarket.Client.Model
{
	public class GetEventsResponse
	{
		public GetEventsResponse()
		{
		}

        public GetEventsResponse(int resultCount, IList<EventRequest> results)
        {
            ResultCount = resultCount;
            Results = results;
        }

        [JsonPropertyName("resultCount")]
        public int ResultCount { get; set; }

        [JsonPropertyName("results")]
        public IList<EventRequest> Results { get; set; } = new List<EventRequest>();
    }
}

