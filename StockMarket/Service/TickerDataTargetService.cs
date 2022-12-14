using System;
using bagend_web_scraper.Repository;
using bagend_web_scraper.StockMarket.Entity;
using bagend_web_scraper.StockMarket.Exceptions;
using bagend_web_scraper.StockMarket.Model;
using bagend_web_scraper.StockMarket.OpenClose;
using bagend_web_scraper.Timer;

namespace bagend_web_scraper.StockMarket.Service
{
	public class TickerDataTargetService
	{

		private readonly TickerDataTargetEntityRepository _tickerDataTargetEntityRepository;
        private readonly ILogger<TickerDataTargetService> _logger;

        public TickerDataTargetService(TickerDataTargetEntityRepository tickerDataTargetEntityRepository,
            ILogger<TickerDataTargetService> logger)
		{
			_tickerDataTargetEntityRepository = tickerDataTargetEntityRepository;
			_logger = logger;
		}

		public TickerDataTarget createTarget(CreateTickerDataTargetRequest request)
		{
			var entity = createTargetEntity(request);

			var saved = createTarget(entity);

			return TickerDataTarget.FromEntity(saved);
        }

        public IList<TickerDataTarget> GetTargets()
        {
            var entities = _tickerDataTargetEntityRepository.GetAsync().Result;
			var results = new List<TickerDataTarget>();
			foreach(TickerDataTargetEntity entity in entities)
			{
				results.Add(TickerDataTarget.FromEntity(entity));
			}

			return results;
        }

        public IList<TickerDataTargetEntity> GetTargetsForScraping()
		{
			return _tickerDataTargetEntityRepository.GetNextWorkAsync().Result;
		}

		public TickerDataTarget GetDataTarget(string stockTicker)
		{
			var entity = _tickerDataTargetEntityRepository.GetByStockTickerAsync(stockTicker).Result;

            return TickerDataTarget.FromEntity(entity);
		}

		public void updateTarget(TickerDataTargetEntity dataTargetEntity)
		{
            _logger.LogInformation("saving updated ticker data target {}", dataTargetEntity.Id);
            var timer = Timer.Timer.TimerFactory(true);
			var entity = dataTargetEntity.withCurrentDateTimeAsUpdateTimestamp();
			_tickerDataTargetEntityRepository.UpdateAsync(entity.Id, entity).Wait();
            _logger.LogInformation("done saving updated ticker data target {}, took {} millis", dataTargetEntity.Id, timer.getTimeElasped());
        }

        public TickerDataTarget updateTarget(TickerDataTarget tickerDataTarget)
        {
            _logger.LogInformation("saving updated ticker data target {}", tickerDataTarget.Id);
            var timer = Timer.Timer.TimerFactory(true);
			var found = _tickerDataTargetEntityRepository.GetAsync(tickerDataTarget.Id).Result;
			if(found != null)
			{
                updateTargetEntity(found, tickerDataTarget);
                var entity = found.withCurrentDateTimeAsUpdateTimestamp();
                _tickerDataTargetEntityRepository.UpdateAsync(entity.Id, entity).Wait();
                _logger.LogInformation("done saving updated ticker data target {}, took {} millis", tickerDataTarget.Id, timer.getTimeElasped());
				return TickerDataTarget.FromEntity(entity);
            }
			throw new TargetNotFoundException(tickerDataTarget.Id);
        }

        private TickerDataTargetEntity createTarget(TickerDataTargetEntity tickerDataTargetEntity)
		{
			_logger.LogInformation("saving ticker data target {}", tickerDataTargetEntity.Id);
			var timer = Timer.Timer.TimerFactory(true);
			var entity = tickerDataTargetEntity.withCurrentDateTimeAsCreationTimestamp();
			entity.LastUpdatedAt = entity.TargetCreatedAt;

			_tickerDataTargetEntityRepository.CreateAsync(entity).Wait();
            _logger.LogInformation("done saving ticker data target {}, took {} millis", tickerDataTargetEntity.Id, timer.getTimeElasped());

			return entity;
        }

		private static TickerDataTargetEntity createTargetEntity(CreateTickerDataTargetRequest request)
		{
			var entity = new TickerDataTargetEntity();
			entity.Priority = request.Priority;
			entity.BusinessSector = request.BusinessSector;
			entity.CompanyName = request.CompanyName;
			entity.TickerSymbol = request.TickerSymbol;

			return entity;
		}

		private static void updateTargetEntity(TickerDataTargetEntity entity, TickerDataTarget tickerDataTarget)
		{
            entity.Id = tickerDataTarget.Id;
            entity.Priority = tickerDataTarget.Priority;
            entity.TickerSymbol = tickerDataTarget.TickerSymbol;
            entity.CompanyName = tickerDataTarget.CompanyName;
            entity.BusinessSector = tickerDataTarget.BusinessSector;
            entity.IsStarted = tickerDataTarget.IsStarted;
            entity.IsCompleted = tickerDataTarget.IsCompleted;
            entity.IsActive = tickerDataTarget.IsActive;
            entity.LastDatapointTimeValue = tickerDataTarget.LastDatapointTimeValue;
        }
	}
}

