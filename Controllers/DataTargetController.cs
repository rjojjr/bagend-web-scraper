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
        private readonly TickerDataScraper _tickerDataScraper;


        public DataTargetController(ILogger<DataTargetController> logger,
            TickerDataTargetService tickerDataTargetService,
            StockDataScraper openCloseStockDataScraper,
            TickerDataScraper tickerDataScraper)
		{
			_logger = logger;
			_tickerDataTargetService = tickerDataTargetService;
            _stockDataScraper = openCloseStockDataScraper;
            _tickerDataScraper = tickerDataScraper;
        }

        /// <summary>
        /// Saves new stocker ticker data target.
        /// </summary>
        /// <description>Saves a new stocker ticker data target. NOTE: This new target will not be in the operation queue.</description>
        /// <remarks></remarks>
        /// <response code="201">Stocker ticker data target created</response>
        /// <response code="500">Something went wrong</response>
        [HttpPost]
        [ApiExplorerSettings(GroupName = "v1")]
        public IActionResult SubmitNewTickerDataTarget(CreateTickerDataTargetRequest request)
		{
            return ExecuteWithExceptionHandler(() => {
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
        [ApiExplorerSettings(GroupName = "v1")]
        public IActionResult GetTickerDataTargets()
        {
            return ExecuteWithExceptionHandler(() => {
                _logger.LogInformation("received request to fetch ticker data targets");
                var results = _tickerDataTargetService.GetTargets();
                return Ok(new TickerDataTargetResults(results.Count(), results));
            });
        }

        /// <summary>
        /// Fetch all active US stock tickers and save them as targets.
        /// </summary>
        /// <remarks></remarks>
        /// <response code="200">Success</response>
        /// <response code="500">Something went wrong</response>
        [HttpGet("tickers/scrape")]
        [ApiExplorerSettings(GroupName = "v1")]
        public IActionResult ScrapeTickerDataTargets()
        {
            return ExecuteWithExceptionHandler(() => {
                _logger.LogInformation("received request to fetch ticker data targets");
                var results = _tickerDataScraper.ScrapeStockTickers();
                return Ok(results);
            });
        }

        /// <summary>
        /// Fetches all saved stocker tickers.
        /// </summary>
        /// <remarks></remarks>
        /// <response code="200">Success</response>
        /// <response code="500">Something went wrong</response>
        [HttpGet("tickers")]
        [ApiExplorerSettings(GroupName = "v1")]
        public IActionResult GetTickers()
        {
            return ExecuteWithExceptionHandler(() => {
                _logger.LogInformation("received request to fetch tickers");
                var results = _tickerDataTargetService.GetTickers();
                return Ok(results);
            });
        }

        /// <summary>
        /// Fetches saved stocker ticker data target.
        /// </summary>
        /// <remarks></remarks>
        /// <response code="200">Success</response>
        /// <response code="500">Something went wrong</response>
        [HttpGet]
        [Route("target")]
        [ApiExplorerSettings(GroupName = "v1")]
        public IActionResult GetTickerDataTarget(string stockTicker)
        {
            return ExecuteWithExceptionHandler(() => {
                _logger.LogInformation("received request to fetch ticker data target {]", stockTicker);
                var results = _tickerDataTargetService.GetDataTarget(stockTicker);
                return Ok(results);
            });
        }

        /// <summary>
        /// Fetches sixe of operations queue.
        /// </summary>
        /// <remarks></remarks>
        /// <response code="200">Success</response>
        /// <response code="500">Something went wrong</response>
        [HttpGet]
        [Route("queue/size")]
        [ApiExplorerSettings(GroupName = "v1")]
        public IActionResult GetOpereationsQueueSize()
        {
            return ExecuteWithExceptionHandler(() => {
                _logger.LogInformation("received request to get operations queue size");
                var results = _tickerDataTargetService.GetOperationQueueSize();
                return Ok(results);
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
        [ApiExplorerSettings(GroupName = "v1")]
        public IActionResult RestartDataOperations()
        {
            return ExecuteWithExceptionHandler(() => {
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
        /// <response code="404">No target found with provided id</response>
        /// <response code="500">Something went wrong</response>
        [HttpPatch]
        [ApiExplorerSettings(GroupName = "v1")]
        public IActionResult UpdateTickerDataTarget(TickerDataTarget tickerDataTarget)
        {
            return ExecuteWithExceptionHandler(() => {
                _logger.LogInformation("received request to update ticker data target {}", tickerDataTarget.Id);
                return Ok(_tickerDataTargetService.updateTarget(tickerDataTarget));
            });
        }
    }
}

