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
        /// Saves new stocker ticker data target.
        /// </summary>
        /// <remarks></remarks>
        /// <response code="201">Stocker ticker data target created</response>
        /// <response code="500">Something went wrong</response>
        [HttpPost]
		public IActionResult SubmitNewTickerDataTarget(CreateTickerDataTargetRequest request)
		{
            return ExecuteWithExceptionHandler<IActionResult>(() => {
                _logger.LogInformation("received request to create new {} ticker data target {}", request.BusinessSector, request.TickerSymbol);

                return Created(".", _tickerDataTargetService.createTarget(request));
            });
		}

        /// <summary>
        /// Fetches all saved stocker ticker data target.
        /// </summary>
        /// <remarks></remarks>
        /// <response code="200">Success</response>
        /// <response code="500">Something went wrong</response>
        [HttpGet]
        public IActionResult GetTickerDataTargets()
        {
            return ExecuteWithExceptionHandler<IActionResult>(() => {
                _logger.LogInformation("received request to fetch ticker data targets");
                var results = _tickerDataTargetService.GetTargets();
                return Ok(new TickerDataTargetResults(results.Count(), results));
            });
        }

        /// <summary>
        /// Restarts data scraping operations. Should be called after submitting new data targets.
        /// </summary>
        /// <remarks></remarks>
        /// <response code="200">Data scraping operations restarted</response>
        /// <response code="500">Something went wrong</response>
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

        /// <summary>
        /// Updates an existing ticker data target.
        /// </summary>
        /// <remarks></remarks>
        /// <response code="200">Stocker ticker data target updated successfully</response>
        /// <response code="500">Something went wrong</response>
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

