using System;
namespace bagend_web_scraper.StockMarket.Exception
{
	public class TargetNotFoundException : SystemException
    {
		public string TargetEntityId { get; set; } = null!;

		public TargetNotFoundException(string id) : base("target entity " + id + " not found")
		{
			TargetEntityId = id;
		}
	}
}

