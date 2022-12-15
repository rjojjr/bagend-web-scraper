using System;
namespace bagend_web_scraper.StockMarket.Model
{
	public class TickerDataTargetResults
	{

		public int ResultsCount { get; set; }
		public IList<TickerDataTarget> Results { get; set; } = new List<TickerDataTarget>();

		public TickerDataTargetResults(int resultsCount, IList<TickerDataTarget> results)
		{
			ResultsCount = resultsCount;
			Results = results;
		}
	}
}

