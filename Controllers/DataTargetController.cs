using System;
using bagend_web_scraper.StockMarket.Model;
using bagend_web_scraper.StockMarket.OpenClose;
using bagend_web_scraper.StockMarket.Service;
using Microsoft.AspNetCore.Mvc;

namespace bagend_web_scraper.Controllers
{

    [ApiController]
    [Route("data/target/api/v1")]
    public class DataTargetController
	{

        private readonly ILogger<DataTargetController> _logger;
		private TickerDataTargetService _tickerDataTargetService;
		private readonly StockDataScraper _stockDataScraper;


        public DataTargetController(ILogger<DataTargetController> logger,
            TickerDataTargetService tickerDataTargetService,
            StockDataScraper openCloseStockDataScraper)
		{
			_logger = logger;
			_tickerDataTargetService = tickerDataTargetService;
            _stockDataScraper = openCloseStockDataScraper;
        }

        [HttpPost]
		public TickerDataTarget SubmitNewTickerDataTarget(CreateTickerDataTargetRequest request)
		{
			_logger.LogInformation("received request to create new {} ticker data target {}", request.BusinessSector, request.TickerSymbol);
			return _tickerDataTargetService.createTarget(request);
		}

        [HttpGet]
        public TickerDataTargetResults GetTickerDataTargets()
        {
            _logger.LogInformation("received request to fetch ticker data targets");
            var results = _tickerDataTargetService.GetTargets();
            return new TickerDataTargetResults(results.Count(), results);
        }

        [HttpPatch]
        [Route("operations/restart")]
        public void RestartDataOperations()
        {
            _logger.LogInformation("received request to restart ticker data operations");
            _stockDataScraper.RestartScraperThread();
        }

        [HttpPatch]
        public TickerDataTarget UpdateTickerDataTarget(TickerDataTarget tickerDataTarget)
        {
            _logger.LogInformation("received request to update ticker data target {}", tickerDataTarget.Id);
            return _tickerDataTargetService.updateTarget(tickerDataTarget);
        }
    }
}

