using System;
using bagend_web_scraper.Repository;
using bagend_web_scraper.StockMarket.Client;
using static bagend_web_scraper.StockMarket.Client.Model.PolygonTickerDataResponse;

namespace bagend_web_scraper.StockMarket.OpenClose
{
	public class TickerDataScraper
	{

        private readonly ILogger<TickerDataScraper> _logger;
        private readonly PolygonApiResponseProcessor _polygonApiResponseProcessor;
        private readonly PolygonApiRESTClient _polygonApiRESTClient;
        private readonly TickerDataTargetEntityRepository _tickerDataTargetEntityRepository;

        public TickerDataScraper(ILogger<TickerDataScraper> logger,
            PolygonApiResponseProcessor polygonApiResponseProcessor,
            PolygonApiRESTClient polygonApiRESTClient,
            TickerDataTargetEntityRepository tickerDataTargetEntityRepository)
        {
            _logger = logger;
            _polygonApiResponseProcessor = polygonApiResponseProcessor;
            _polygonApiRESTClient = polygonApiRESTClient;
            _tickerDataTargetEntityRepository = tickerDataTargetEntityRepository;
        }

        public int ScrapeStockTickers()
        {
            var response = _polygonApiRESTClient.GetTickers();

            var filtered = new List<TickerResult>();
            foreach(TickerResult tickerResult in response.Results)
            {
                if(tickerResult.Locale == "us")
                {
                    filtered.Add(tickerResult);
                }
            }

            response.Results = filtered;

            var entities = _polygonApiResponseProcessor.ProcessPolygonTickerDataRespons(response);


            _tickerDataTargetEntityRepository.CreateMany(entities);
            return entities.Count();
        }
    }
}

