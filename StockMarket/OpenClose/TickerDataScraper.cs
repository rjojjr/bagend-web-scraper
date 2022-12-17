using System;
using bagend_web_scraper.Repository;
using bagend_web_scraper.StockMarket.Client;
using bagend_web_scraper.StockMarket.Client.Model;
using bagend_web_scraper.StockMarket.Entity;

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

        public ScrapeTickersResponse ScrapeStockTickers()
        {

            var entities =  new List<TickerDataTargetEntity>();
            var responses = _polygonApiRESTClient.GetTickers();

           foreach(PolygonTickerDataResponse response in responses)
            {
                var filtered = new List<TickerResult>();
                foreach (TickerResult tickerResult in response.Results)
                {
                    if (tickerResult.Locale == "us")
                    {
                        filtered.Add(tickerResult);
                    }
                }

                response.Results = filtered;

                entities.AddRange(_polygonApiResponseProcessor.ProcessPolygonTickerDataRespons(response));
            }

            var savedEntities = new List<TickerDataTargetEntity>();
            foreach (TickerDataTargetEntity entity in entities)
            {
                try
                {
                    _tickerDataTargetEntityRepository.CreateAsync(entity).Wait();
                    savedEntities.Add(entity);
                }
                catch (Exception e)
                {
                    _logger.LogError("an error occured while saving ", e.StackTrace);
                }

            }
            return new ScrapeTickersResponse(savedEntities.Count(), entities.Count() - savedEntities.Count());
        }
    }

    public class ScrapeTickersResponse
    {
        public ScrapeTickersResponse(int added, int failed)
        {
            Added = added;
            Failed = failed;
        }

        public ScrapeTickersResponse()
        {
        }

        public int Added { get; set; }
        public int Failed { get; set; }
    }
}

