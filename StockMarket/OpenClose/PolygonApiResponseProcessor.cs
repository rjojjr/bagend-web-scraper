using System;
namespace bagend_web_scraper.StockMarket.Client
{
	public class PolygonApiResponseProcessor
	{
		public PolygonApiResponseProcessor()
		{
		}

		public EventRequest ProcessPolygonOpenCloseResponse(PolygonOpenCloseApiResponse response)
		{
			var eventRequest = new EventRequest();
			eventRequest.EventStream = "stock-data-events";
			eventRequest.EventName = "open-close-data-submission";
			eventRequest.EventType = "data-submission";
			eventRequest.EventAttributes = BuildEventAttributes(response);

			return eventRequest;
		}

		private static IList<EventAttribute> BuildEventAttributes(PolygonOpenCloseApiResponse response)
		{
			var attributes = new List<EventAttribute>();
			attributes.Add(new EventAttribute("AfterHours", response.AfterHours.ToString()));
            attributes.Add(new EventAttribute("Close", response.Close.ToString()));
            attributes.Add(new EventAttribute("High", response.High.ToString()));
            attributes.Add(new EventAttribute("Low", response.Low.ToString()));
            attributes.Add(new EventAttribute("Open", response.Open.ToString()));
            attributes.Add(new EventAttribute("Volume", response.Volume.ToString()));

            return attributes;
		}
	}
}

