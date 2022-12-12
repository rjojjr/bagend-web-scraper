using System;
namespace bagend_web_scraper.StockMarket.Service
{
	public class DateProvider
	{

		private readonly ILogger<DateProvider> _logger;

        public DateProvider(ILogger<DateProvider> logger)
        {
			_logger = logger;
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
                DateTime dt = GetDateTimeFromString(currentDate);

                if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday)
				{
                    _logger.LogTrace("target date {} found for date {}", currentDate, date);
                    dates.Add(currentDate);
                }
                currentDate = GetNextDateString(currentDate);
            }

			return dates;
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

