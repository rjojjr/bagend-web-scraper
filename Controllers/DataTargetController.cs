using System;
using System.Net;
using bagend_web_scraper.StockMarket.Model;
using bagend_web_scraper.StockMarket.OpenClose;
using bagend_web_scraper.StockMarket.Service;
using Microsoft.AspNetCore.Mvc;

namespace bagend_web_scraper.Controllers
{

    [ApiController]
    [Route("data/target/api/v1")]
    public class DataTargetController : BaseController
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

        /// <summary>
        /// Submits new stocker ticker data target.
        /// </summary>
        /// <remarks></remarks>
        /// <response code="201">Stocker ticker data target created</response>
        /// <response code="400">Product has missing/invalid values</response>
        /// <response code="500">Oops! Can't create your product right now</response>
        [HttpPost]
		public IActionResult SubmitNewTickerDataTarget(HttpRequestMessage httpRequest, CreateTickerDataTargetRequest request)
		{
            return ExecuteWithExceptionHandler<IActionResult>(() => {
                _logger.LogInformation("received request to create new {} ticker data target {}", request.BusinessSector, request.TickerSymbol);

                return Created(".", _tickerDataTargetService.createTarget(request));
            });
		}

        [HttpGet]
        public IActionResult GetTickerDataTargets()
        {
            return ExecuteWithExceptionHandler<IActionResult>(() => {
                _logger.LogInformation("received request to fetch ticker data targets");
                var results = _tickerDataTargetService.GetTargets();
                return Ok(new TickerDataTargetResults(results.Count(), results));
            });
        }

        [HttpPatch]
        [Route("operations/restart")]
        public IActionResult RestartDataOperations()
        {
            return ExecuteWithExceptionHandler<IActionResult>(() => {
                _logger.LogInformation("received request to restart ticker data operations");
                _stockDataScraper.RestartScraperThread();
                return Ok();
            });
        }

        [HttpPatch]
        public IActionResult UpdateTickerDataTarget(TickerDataTarget tickerDataTarget)
        {
            return ExecuteWithExceptionHandler<IActionResult>(() => {
                _logger.LogInformation("received request to update ticker data target {}", tickerDataTarget.Id);
                return Ok(_tickerDataTargetService.updateTarget(tickerDataTarget));
            });
        }
    }
}

