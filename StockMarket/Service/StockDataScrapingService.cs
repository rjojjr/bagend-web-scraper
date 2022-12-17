using System;
using Microsoft.Extensions.Hosting;

namespace bagend_web_scraper.StockMarket.Service
{
    public class StockDataScrapingService : IHostedService
    {

        private readonly ILogger<StockDataScrapingService> _logger;
        private readonly StockDataScraper _stockDataScraper;

        public StockDataScrapingService(ILogger<StockDataScrapingService> logger,
            StockDataScraper stockDataScraper)
        {
            _logger = logger;
            _stockDataScraper = stockDataScraper;
        }

        public async Task StartScraper(CancellationToken cancellationToken)
        { 
            await Task.Run(() =>
            {
                //_stockDataScraper.RunScraperThread();
            });
        }


        //Register hosted service 
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<StockDataScrapingService>();
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("starting scraper thread");
            return StartScraper(cancellationToken);
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                _logger.LogInformation("cleaning up after scraper thread");
            });
        }
    }
 
}

