using System;
using bagend_web_scraper.StockMarket.Model;
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

		public DataTargetController(ILogger<DataTargetController> logger,
            TickerDataTargetService tickerDataTargetService)
		{
			_logger = logger;
			_tickerDataTargetService = tickerDataTargetService;
		}

        [HttpPost]
		public TickerDataTarget SubmitNewTickerDataTarget(CreateTickerDataTargetRequest request)
		{
			_logger.LogInformation("received request to create new {} ticker data target {}", request.BusinessSector, request.TickerSymbol);
			return _tickerDataTargetService.createTarget(request);
		}
    }
}

