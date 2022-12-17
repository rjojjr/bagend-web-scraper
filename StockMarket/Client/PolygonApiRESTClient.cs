using System;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using bagend_web_scraper.Config;
using bagend_web_scraper.StockMarket.Client.Model;
using Microsoft.Extensions.Options;
using RestSharp;

namespace bagend_web_scraper.StockMarket.Client
{
	public class PolygonApiRESTClient
	{
        private readonly ILogger<PolygonApiRESTClient> _logger;
        private readonly IOptions<PolygonApiConfig> _apiConfig;
        private readonly RestClient _restClient;
        private PolygonOpenCloseApiResponse _lastResponse = new PolygonOpenCloseApiResponse();

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
            try
            {
                _logger.LogInformation("fetching stock data for ticker {} on {}", tickerSymbol, date);
                var timer = Timer.Timer.TimerFactory(true);
                var result = GetOpenCloseAsync(tickerSymbol, date).Result;

                if(result.From == null)
                {
                    throw new Exception();
                }

                _logger.LogInformation("done fetching stock data for ticker {} on {}, took {} millis", tickerSymbol, date, timer.getTimeElasped());
                _lastResponse = result;
                return result;
            }
            catch (Exception e)
            {
                if (_lastResponse.Symbol != null && _lastResponse.Symbol.Equals(tickerSymbol))
                {
                    _lastResponse.From = date;
                    return _lastResponse;
                }
                _lastResponse = new PolygonOpenCloseApiResponse();
                _lastResponse.Symbol = tickerSymbol;
                _lastResponse.From = date;
                return _lastResponse;
            }
        }

        public IList<PolygonTickerDataResponse> GetTickers()
        {
            _logger.LogInformation("fetching all stock tickers");
            var timer = Timer.Timer.TimerFactory(true);
            var responses = new List<PolygonTickerDataResponse>();
            var resp = GetTickersAsync().Result;
            responses.Add(resp);
            while(resp.NextUrl != null && resp.NextUrl != "")
            {
                _logger.LogInformation("fetching next stock tickers");
               try
                {
                    resp = GetNextTickers(resp.NextUrl.Split("https://api.polygon.io")[1]).Result;
                    responses.Add(resp);
                }
                catch (Exception e)
                { 
                    _logger.LogError("error fetching next items");
                    _logger.LogError(e.StackTrace);
                    break;
                }
            }

            _logger.LogInformation("done fetching all stock tickers, took {} millis", timer.getTimeElasped());
            return responses;
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

        private async Task<PolygonTickerDataResponse> GetTickersAsync()
        {
            var request = new RestRequest("/v3/reference/tickers?type=CS&market=stocks&active=true&limit=1000&apiKey=" + _apiConfig.Value.ApiKey);
            return await _restClient.GetAsync<PolygonTickerDataResponse>(request);
        }

        private async Task<PolygonTickerDataResponse> GetNextTickers(string url)
        {
            var request = new RestRequest(url + "&apiKey=" + _apiConfig.Value.ApiKey);
            return await _restClient.GetAsync<PolygonTickerDataResponse>(request);
        }
    }
}

