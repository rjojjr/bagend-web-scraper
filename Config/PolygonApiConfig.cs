using System;
namespace bagend_web_scraper.Config
{
	public class PolygonApiConfig
	{
        public string Url { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
		public long ThrottleMilliseconds { get; set; } = 0;
        public int MaxQueueLength { get; set; } = 0;
        public int MaxThreads { get; set; } = 1;

        public PolygonApiConfig()
		{
		}
	}
}

