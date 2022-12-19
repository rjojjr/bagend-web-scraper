using System;
using bagend_web_scraper.Config;
using bagend_web_scraper.StockMarket.Client.Model;
using bagend_web_scraper.StockMarket.Service;
using Microsoft.Extensions.Options;
using RestSharp;

namespace bagend_web_scraper.StockMarket.Client
{
	public class EventApiRESTClient
	{
        private readonly ILogger<EventApiRESTClient> _logger;
        private readonly IOptions<EventApiConfig> _apiConfig;
        private readonly RestClient _restClient;

        public EventApiRESTClient(IOptions<EventApiConfig> apiConfig, ILogger<EventApiRESTClient> logger)
        {
            _logger = logger;
            _apiConfig = apiConfig;
            _restClient = ApiClientFactory(apiConfig);
        }

        private static RestClient ApiClientFactory(IOptions<EventApiConfig> apiConfig)
        {
            var options = new RestClientOptions(apiConfig.Value.Url)
            {
                ThrowOnAnyError = true
            };
            return new RestClient(options);
        }

        public void SubmitEvent(EventRequest eventRequest)
        {
            _logger.LogDebug("submitting event to api stream {}", eventRequest.EventStream);
            var timer = Timer.Timer.TimerFactory(true);
            SubmitEventAsync(eventRequest).Wait();
            _logger.LogDebug("done submitting event to api stream {}, took {} millis", eventRequest.EventStream, timer.getTimeElasped());
        }

        public GetEventsResponse GetEventsByAttributeValue(string attributeName, string attributeValue)
        {
            return GetEventsByAttributeValueAsync(attributeName, attributeValue).Result;
        }

        private async Task<string> SubmitEventAsync(EventRequest eventRequest)
        { 
            var request = new RestRequest("/generic/events/api/v1").AddBody(eventRequest);
            return await _restClient.PostAsync<string>(request);
        }



        private async Task<GetEventsResponse> GetEventsByAttributeValueAsync(string attributeName, string attributeValue)
        {
            var request = new RestRequest("/generic/events/api/v1/attribute?AttributeName=" + attributeName + "&AttributeValue=" + attributeValue);
            return await _restClient.GetAsync<GetEventsResponse>(request);
        }
    }
}

