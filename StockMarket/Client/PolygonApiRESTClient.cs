using System;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Text.Json.Serialization;
using System.Threading;
using bagend_web_scraper.Config;
using Microsoft.Extensions.Options;
using RestSharp;

namespace bagend_web_scraper.StockMarket.Client
{
	public class PolygonApiRESTClient
	{
        private readonly ILogger<PolygonApiRESTClient> _logger;
        private readonly IOptions<PolygonApiConfig> _apiConfig;
        private readonly RestClient _restClient;

        public PolygonApiRESTClient(IOptions<PolygonApiConfig> apiConfig, ILogger<PolygonApiRESTClient> logger)
		{
            _logger = logger;
            _apiConfig = apiConfig;
            _restClient = ApiClientFactory(apiConfig);
		}

        private static RestClient ApiClientFactory(IOptions<PolygonApiConfig> apiConfig)
        {
            var options = new RestClientOptions(apiConfig.Value.Url)
            {
                ThrowOnAnyError = true
            };
           return new RestClient(options);
        }

        public PolygonOpenCloseApiResponse GetOpenClose(string tickerSymbol, string date)
        {
            _logger.LogInformation("fetching stock data for ticker {} on {}", tickerSymbol, date);
            var timer = Timer.Timer.TimerFactory(true);
            var result = GetOpenCloseAsync(tickerSymbol, date).Result;
            _logger.LogInformation("done fetching stock data for ticker {} on {}, took {} millis", tickerSymbol, date, timer.getTimeElasped());
            return result;
        }

        public long GetThrottlePeriod()
        {
            return _apiConfig.Value.ThrottleMilliseconds;
        }

        private async Task<PolygonOpenCloseApiResponse> GetOpenCloseAsync(string tickerSymbol, string date)
        {
            var request = new RestRequest("/v1/open-close/" + tickerSymbol + "/" + date + "?apiKey=" + _apiConfig.Value.ApiKey);
            return await _restClient.GetAsync<PolygonOpenCloseApiResponse>(request);
        }
    }
}

