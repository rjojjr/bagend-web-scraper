using System;
using bagend_web_scraper.StockMarket.Client;
using bagend_web_scraper.StockMarket.Operations;

namespace bagend_web_scraper.StockMarket.OpenClose
{
	public class OpenCloseStockDataScraper
	{

        private readonly ILogger<OpenCloseStockDataScraper> _logger;
        private readonly PolygonApiResponseProcessor _polygonApiResponseProcessor;
		private readonly PolygonApiRESTClient _polygonApiRESTClient;
		private readonly EventApiRESTClient _eventApiRESTClient;
		private readonly OperationProcessor _throttledOperationProcessor;

		public OpenCloseStockDataScraper(ILogger<OpenCloseStockDataScraper> logger,
			PolygonApiResponseProcessor polygonApiResponseProcessor,
            PolygonApiRESTClient polygonApiRESTClient,
            EventApiRESTClient eventApiRESTClient,
            OperationProcessor throttledOperationProcessor)
		{
			_logger = logger;
			_polygonApiResponseProcessor = polygonApiResponseProcessor;
			_polygonApiRESTClient = polygonApiRESTClient;
			_eventApiRESTClient = eventApiRESTClient;
			_throttledOperationProcessor = throttledOperationProcessor;
			_throttledOperationProcessor.StartOperationProcessingThread();
		}

		public void RestartOperationsQueue()
		{
			_logger.LogInformation("restarting open/close operations queue");
			_throttledOperationProcessor.StartOperationProcessingThread();
		}

		public void ScheduleScrapeDataOperation(string stockTicker, string date, ThreadStart successCallBack, ThreadStart failureCallBack)
		{
            _logger.LogInformation("scheduling scrape stock data for ticker {} at {}", stockTicker, date);
            _throttledOperationProcessor.QueueOperation(() => ScrapeData(stockTicker, date, successCallBack, failureCallBack));
		}

		private bool ScrapeData(string stockTicker, string date, ThreadStart successCallBack, ThreadStart failureCallBack)
		{
			_logger.LogInformation("scraping stock data for ticker {} at {}", stockTicker, date);
			var timer = Timer.Timer.TimerFactory(true);
			var result = ScrapeAndProcessTickerDataPoint(stockTicker, date);
			var timeElapsed = timer.getTimeElasped();
			if (result)
			{
				_logger.LogInformation("done scraping stock data for ticker {} at {}, took {} millis", stockTicker, date, timeElapsed);
				InvokeCallBack(successCallBack);
			}
			else
			{
                _logger.LogError("failed scraping stock data for ticker {} at {}, took {} millis", stockTicker, date, timeElapsed);
				InvokeCallBack(failureCallBack);
            }

			return result;
        }

		private bool ScrapeAndProcessTickerDataPoint(string stockTicker, string date)
		{
            try
            {
                var polygonResponse = _polygonApiRESTClient.GetOpenClose(stockTicker, date);
                var eventRequest = _polygonApiResponseProcessor.ProcessPolygonOpenCloseResponse(polygonResponse);
                _eventApiRESTClient.SubmitEvent(eventRequest);
                return true;
            }
			catch (Exception e)
			{
				_logger.LogError("error scraping and processing stock data {}", e.Message);
			}
			return false;
		}

		private static void InvokeCallBack(ThreadStart callback)
		{
			if(callback != null)
			{
				callback.Invoke();
			}
		}
	}
}

