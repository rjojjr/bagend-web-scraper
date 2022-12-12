using System;
namespace bagend_web_scraper.StockMarket.Model
{
	public class CreateTickerDataTargetRequest
	{
        public int Priority { get; set; } = 100;

        public string TickerSymbol { get; set; } = null!;

        public string BusinessSector { get; set; } = null!;

        public string CompanyName { get; set; } = null!;

        public CreateTickerDataTargetRequest()
		{

		}
	}
}

