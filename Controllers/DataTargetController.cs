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
		private readonly OpenCloseStockDataScraper _openCloseStockDataScraper;


        public DataTargetController(ILogger<DataTargetController> logger,
            TickerDataTargetService tickerDataTargetService,
            OpenCloseStockDataScraper openCloseStockDataScraper)
		{
			_logger = logger;
			_tickerDataTargetService = tickerDataTargetService;
            _openCloseStockDataScraper = openCloseStockDataScraper;
        }

        [HttpPost]
		public TickerDataTarget SubmitNewTickerDataTarget(CreateTickerDataTargetRequest request)
		{
			_logger.LogInformation("received request to create new {} ticker data target {}", request.BusinessSector, request.TickerSymbol);
			return _tickerDataTargetService.createTarget(request);
		}

        [HttpGet]
        [Route("operations/restart")]
        public void RestartDataOperations()
        {
            _logger.LogInformation("received request to restart ticker data operations");
            _openCloseStockDataScraper.RestartOperationsQueue();
        }
    }
}

