using System;
using System.Text.Json.Serialization;

namespace bagend_web_scraper.StockMarket.Client
{
	public class EventRequest
	{
        [JsonPropertyName("eventStream")]
        public string EventStream { get; set; } = null!;

        [JsonPropertyName("eventType")]
        public string EventType { get; set; } = null!;

        [JsonPropertyName("eventName")]
        public string EventName { get; set; } = null!;

        [JsonPropertyName("eventAttributes")]
        public IList<EventAttribute> EventAttributes { get; set; } = new List<EventAttribute>();

        public EventRequest()
		{
		}
	}
}

