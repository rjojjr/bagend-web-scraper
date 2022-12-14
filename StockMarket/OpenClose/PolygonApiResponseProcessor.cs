using System;
using bagend_web_scraper.StockMarket.Client.Model;
using bagend_web_scraper.StockMarket.Entity;
using bagend_web_scraper.StockMarket.Service;

namespace bagend_web_scraper.StockMarket.Client
{
	public class PolygonApiResponseProcessor
	{

		private readonly DateProvider _dateProvider;

		public PolygonApiResponseProcessor(DateProvider dateProvider)
		{
			_dateProvider = dateProvider;
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

		public IList<TickerDataTargetEntity> ProcessPolygonTickerDataRespons(PolygonTickerDataResponse response)
		{
			var entities = new List<TickerDataTargetEntity>();
			foreach(TickerResult tickerResult in response.Results)
			{
                var entity = new TickerDataTargetEntity();
                entity.Priority = 100;
                entity.TickerSymbol = tickerResult.Ticker;
                entity.CompanyName = tickerResult.Name;
                entity.BusinessSector = "UNKNOWN";

				entities.Add(entity);
            }
			return entities;

		}

		private IList<EventAttribute> BuildEventAttributes(PolygonOpenCloseApiResponse response)
		{
			var attributes = new List<EventAttribute>();
			attributes.Add(new EventAttribute("AfterHours", response.AfterHours.ToString()));
            attributes.Add(new EventAttribute("Close", response.Close.ToString()));
            attributes.Add(new EventAttribute("High", response.High.ToString()));
            attributes.Add(new EventAttribute("Low", response.Low.ToString()));
            attributes.Add(new EventAttribute("Open", response.Open.ToString()));
            attributes.Add(new EventAttribute("Volume", response.Volume.ToString()));
            attributes.Add(new EventAttribute("Symbol", response.Symbol.ToString()));
            attributes.Add(new EventAttribute("Date", response.From.ToString()));
			attributes.Add(new EventAttribute("DayOfWeek", _dateProvider.GetDayOfWeek(response.From.ToString())));

            return attributes;
		}
	}
}

