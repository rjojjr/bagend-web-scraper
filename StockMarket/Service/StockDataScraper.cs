using System;
using System.Runtime.CompilerServices;
using bagend_web_scraper.StockMarket.Entity;
using bagend_web_scraper.StockMarket.OpenClose;
using bagend_web_scraper.Threads;

namespace bagend_web_scraper.StockMarket.Service
{
	public class StockDataScraper
	{

		private const string _startDate = "2020-12-31";

        private readonly OpenCloseStockDataScraper _openCloseStockDataScraper;
		private readonly DateProvider _dateProvider;
		private readonly TickerDataTargetService _tickerDataTargetService;
        private readonly ILogger<StockDataScraper> _logger;

		private readonly ThreadTracker _scraperThreadTracker;
        private Thread _scraperThread;

        public StockDataScraper(OpenCloseStockDataScraper openCloseStockDataScraper,
			DateProvider dateProvider,
			TickerDataTargetService tickerDataTargetService,
			ILogger<StockDataScraper> logger)
		{
			_openCloseStockDataScraper = openCloseStockDataScraper;
			_dateProvider = dateProvider;
			_tickerDataTargetService = tickerDataTargetService;
			_logger = logger;
			_scraperThreadTracker = new ThreadTracker();
		}

		public void RunScraperThread()
		{
			_logger.LogInformation("starting stock data scraper thread...");
            ThreadStart threadDelegate = new ThreadStart(() =>
            {
                var timer = Timer.Timer.TimerFactory(true);
                _scraperThreadTracker.ActivateThread();
                _logger.LogInformation("started stock data scraper thread");
                var work = _tickerDataTargetService.GetTargetsForScraping();
                foreach (TickerDataTargetEntity entity in work)
                {
                    _logger.LogInformation("submitting target {} for scraping", entity.Id);
                    ScrapeOpenCloseStockData(entity);
                }
                _logger.LogInformation("stock data scraper thread finished after {} millis", timer.getTimeElasped());
                _scraperThreadTracker.ActivateThread();
            });
            _scraperThread = new Thread(threadDelegate);
            _scraperThread.Start();
		}

        public void RestartScraperThread()
        {
            _logger.LogInformation("restarting scrapper thread");
            StopScraperThread();
            RunScraperThread();
        }

        private void StopScraperThread()
        {
            if(_scraperThread != null)
            {
                _logger.LogInformation("stopping scraper thread");
                _scraperThread.Abort();
                _scraperThread = null;
            }
        }


        public void  ScrapeOpenCloseStockData(TickerDataTargetEntity entity)
        {
            var dates = _dateProvider.GetNextDateStringsUntilToday(entity.LastDatapointTimeValue != null ? entity.LastDatapointTimeValue : _startDate);
            _logger.LogInformation("found {} dates for target {}", dates.Count(), entity.TickerSymbol);
            var operations = new List<ThreadStart>();

            ProcessAndUpdateTargetStatus(entity, dates);

            foreach (string date in dates)
            {
                ThreadStart successCallback = () =>
                {
                    entity.LastDatapointTimeValue = date;
                    if (date.Equals(dates.Last()))
                    {
                        entity.IsActive = false;
                        entity.IsStarted = false;
                        entity.IsCompleted = true;
                    }
                    _tickerDataTargetService.updateTarget(entity);
                };
                ThreadStart failureCallback = () =>
                {
                    _logger.LogError("error thrown while scraping stock data for ticker {} at date {}", entity.TickerSymbol, date);
                };
                _openCloseStockDataScraper.ScheduleScrapeDataOperation(entity.TickerSymbol, date, successCallback, failureCallback);
            }
        }

        private void ProcessAndUpdateTargetStatus(TickerDataTargetEntity entity, IList<string> dates)
        {
            _logger.LogInformation("updating status for target {}", entity.TickerSymbol);
            entity.IsActive = dates.Count() > 0 ? true : false;
            entity.IsStarted = true;
            entity.IsCompleted = dates.Count() == 0 ? true : false;
            _tickerDataTargetService.updateTarget(entity);
        }

        private IList<string> GetNextDaysUntilToday(string date)
		{
			var dates = new List<string>();

			return dates;
		}
	}
}

