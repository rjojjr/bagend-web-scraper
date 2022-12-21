using System;
using System.Runtime.CompilerServices;
using bagend_web_scraper.StockMarket.Entity;
using bagend_web_scraper.StockMarket.Model;
using bagend_web_scraper.StockMarket.OpenClose;
using bagend_web_scraper.StockMarket.Operations;
using bagend_web_scraper.Threads;
using SharpCompress.Common;

namespace bagend_web_scraper.StockMarket.Service
{
	public class StockDataScraper
	{

		private const string _startDate = "2017-01-01";

        private readonly OpenCloseStockDataScraper _openCloseStockDataScraper;
		private readonly DateProvider _dateProvider;
		private readonly TickerDataTargetService _tickerDataTargetService;
        private readonly ILogger<StockDataScraper> _logger;
        private readonly OperationProcessor _operationProcessor;

		private readonly ThreadTracker _scraperThreadTracker;
        private Thread _scraperThread;
        private IList<string> _datesUntilToday;

        private long total;
        private long completed;
        private long started;

        public StockDataScraper(OpenCloseStockDataScraper openCloseStockDataScraper,
			DateProvider dateProvider,
			TickerDataTargetService tickerDataTargetService,
            OperationProcessor operationProcessor,
            ILogger<StockDataScraper> logger)
		{
			_openCloseStockDataScraper = openCloseStockDataScraper;
			_dateProvider = dateProvider;
			_tickerDataTargetService = tickerDataTargetService;
            _operationProcessor = operationProcessor;
			_logger = logger;
			_scraperThreadTracker = new ThreadTracker();
            _datesUntilToday = _dateProvider.GetNextDateStringsUntilToday(_startDate);
        }

        public ScraperStatus GetStatus()
        {
            long elapsed = DateTime.UtcNow.Millisecond - started;
            long remaining = total - completed;

            decimal rate = completed / elapsed;

            decimal remainingTime = ((rate * remaining * 1000) / 60);
            return new ScraperStatus(total, completed, elapsed, rate, remainingTime);
        }

        public void RunScraperThread()
        {
            _operationProcessor.ResetQueue();
			_logger.LogInformation("starting stock data scraper thread...");
            ThreadStart threadDelegate = new ThreadStart(() =>
            {
                var timer = Timer.Timer.TimerFactory(true);
                _scraperThreadTracker.ActivateThread();
                _logger.LogInformation("started stock data scraper thread");
                var work = _tickerDataTargetService.GetTargetsForScraping();
                started = DateTime.UtcNow.Millisecond;
                total = work.Count();
                var thread1 = ((List<TickerDataTargetEntity>)work).GetRange(0, work.Count() / 3);
                var thread2 = ((List<TickerDataTargetEntity>)work).GetRange(work.Count() / 3, (work.Count() / 3));
                var thread3 = ((List<TickerDataTargetEntity>)work).GetRange((2 * work.Count() / 3), (work.Count() / 3));
                new Thread(new ThreadStart(() =>
                {
                    foreach (TickerDataTargetEntity entity in thread1)
                    {
                        _logger.LogInformation("submitting target {} for scraping", entity.Id);
                        ScrapeOpenCloseStockData(entity);
                    }
                })).Start();

                new Thread(new ThreadStart(() =>
                {
                    foreach (TickerDataTargetEntity entity in thread2)
                    {
                        _logger.LogInformation("submitting target {} for scraping", entity.Id);
                        ScrapeOpenCloseStockData(entity);
                    }
                })).Start();

                new Thread(new ThreadStart(() =>
                {
                    foreach (TickerDataTargetEntity entity in thread3)
                    {
                        _logger.LogInformation("submitting target {} for scraping", entity.Id);
                        ScrapeOpenCloseStockData(entity);
                    }
                })).Start();

                _logger.LogInformation("stock data scraper thread finished after {} millis", timer.getTimeElasped());
                _scraperThreadTracker.ActivateThread();

            });
            _scraperThread = new Thread(threadDelegate);
            _scraperThread.Start();
            _operationProcessor.StartOperationProcessingThread();
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
                try
                {
                    _scraperThread.Interrupt();
                }
                catch (Exception e)
                {
                    _logger.LogError("error interupting stock scraper thread {}", e.Message);
                }
                _scraperThread = null;
            }
            _scraperThreadTracker.DeactivateThread();
        }


        public void  ScrapeOpenCloseStockData(TickerDataTargetEntity entity)
        {
            var dates = _dateProvider.FilterExistingEvents(_datesUntilToday, entity.TickerSymbol);
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
                        Interlocked.Increment(ref completed);
                    }
                    _tickerDataTargetService.updateTarget(entity);
                };
                ThreadStart failureCallback = () =>
                {
                    Interlocked.Increment(ref completed);
                    _logger.LogError("error thrown while scraping stock data for ticker {} at date {}", entity.TickerSymbol, date);
                };
                _openCloseStockDataScraper.ScheduleScrapeDataOperation(entity.TickerSymbol, date, successCallback, failureCallback);
            }
        }

        private void ProcessAndUpdateTargetStatus(TickerDataTargetEntity entity, IList<string> dates)
        {
            _logger.LogDebug("updating status for target {}", entity.TickerSymbol);
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

