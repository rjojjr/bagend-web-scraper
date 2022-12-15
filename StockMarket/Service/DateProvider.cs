using System;
using bagend_web_scraper.StockMarket.Client;

namespace bagend_web_scraper.StockMarket.Service
{
	public class DateProvider
	{

		private readonly ILogger<DateProvider> _logger;
		private readonly EventApiRESTClient _eventApiRESTClient;

        public DateProvider(ILogger<DateProvider> logger,
            EventApiRESTClient polygonApiRESTClient)
        {
			_logger = logger;
			_eventApiRESTClient = polygonApiRESTClient;
        }

		public IList<string> FilterExistingEvents(IList<string> dates, string tickerSymbol)
		{
			var polygons = _eventApiRESTClient.GetEventsByAttributeValue("Symbol", tickerSymbol);
			var existing = ExtractDates(polygons.Results);
			var newDates = new List<string>();
			foreach(string date in dates)
			{
				if(!existing.Contains(date))
				{
					newDates.Add(date);
				}
			}
			return newDates;
		}

        public IList<string> ExtractDates(IList<EventRequest> responses)
        {
            var dates = new List<string>();
            foreach (EventRequest resp in responses)
            {
                dates.Add(extractEventAttribute("Symbol", resp));
            }
            return dates;
        }

        private static string extractEventAttribute(string attributeName, EventRequest genericEvent)
        {
            foreach (EventAttribute attribute in genericEvent.EventAttributes)
            {
                if (attribute.EventAttributeName.ToLower().Equals(attributeName.ToLower()))
                {
                    return attribute.EventAttributeValue.EventAttributeValue;
                }
            }
            return null;
        }


        public IList<string> GetNextDateStringsUntilToday(string date)
		{
			_logger.LogInformation("building target date list for date {}", date);
			var today = GetDateString(DateTime.Today);
            _logger.LogTrace("building target date list for date {} against {}(TODAY)", date, today);

            var dates = new List<string>();
			var currentDate = GetNextDateString(date);
            while (compareDateStrings(currentDate, today) == -1)
            {
                if (IsWeekDay(currentDate))
				{
                    _logger.LogTrace("target date {} found for date {}", currentDate, date);
                    dates.Add(currentDate);
                }
                currentDate = GetNextDateString(currentDate);
            }

			return dates;
        }

        public string GetDayOfWeek(string date)
        {
            DateTime dt = GetDateTimeFromString(date);

            return dt.DayOfWeek.ToString();
        }

        private DateTime GetDateTimeFromString(string date)
		{
			var parts = date.Split("-");
			var year = Int32.Parse(parts[0]);
			var month = Int32.Parse(parts[1]);
			var day = Int32.Parse(parts[2]);

			return new DateTime(year, month, day);
		}

		private string GetDateString(DateTime dateTime)
		{
            return dateTime.Year + "-" + padNumber(dateTime.Month) + "-" + padNumber(dateTime.Day);
        }

		private bool IsWeekDay(string date)
		{
            DateTime dt = GetDateTimeFromString(date);

			return dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday;
        }

        private static string padNumber(int num)
		{
			return num < 10 ? "0" + num : "" + num;
		}

		private string GetNextDateString(string date)
		{
			var dateTime = GetDateTimeFromString(date);
			dateTime = dateTime.AddDays(1);

			return GetDateString(dateTime);
		}

		private int compareDateStrings(string first, string second)
		{
			var firstDt = GetDateTimeFromString(first);
			var secondDt = GetDateTimeFromString(second);

			if(GetMillisFromDateTime(firstDt) < GetMillisFromDateTime(secondDt))
			{
				return -1;
			}
			else if(GetMillisFromDateTime(firstDt) > GetMillisFromDateTime(secondDt))
			{
				return 1;
			}
			return 0;
		}

		public static long GetMillisFromDateTime(DateTime dateTime)
		{
			return (long)dateTime.ToUniversalTime().Subtract(
				new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
				).TotalMilliseconds;
        }
	}
}

