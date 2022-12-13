﻿using System;
using System.Collections.Concurrent;
using bagend_web_scraper.Config;
using Microsoft.Extensions.Options;

namespace bagend_web_scraper.StockMarket.Operations
{
	public class ThrottledFIFOOperationProcessor : OperationProcessor
	{

		private long _lastStart { get; set; } = 0;
		private long _throttlePeriod { get; set; } = 0;
		private int _maxQueueSize { get; set; } = 0;
		private  Thread _processingThread { get; set; } = null!;
		private ConcurrentQueue<ThreadStart> _operationQueue;
		private readonly ILogger<ThrottledFIFOOperationProcessor> _logger;

        public ThrottledFIFOOperationProcessor(IOptions<PolygonApiConfig> polygonApiConfig)
		{
			_throttlePeriod = polygonApiConfig.Value.ThrottleMilliseconds;
			_maxQueueSize = 1000;
            _operationQueue = new ConcurrentQueue<ThreadStart>();
            _logger = LoggerFactory.Create(b => b.AddConsole()).CreateLogger<ThrottledFIFOOperationProcessor>();
        }

		public void ResetQueue()
		{
            _operationQueue = new ConcurrentQueue<ThreadStart>();
        }

		public void QueueOperation(ThreadStart operation)
		{
			if(GetSize() >= _maxQueueSize)
			{
				_logger.LogWarning("cannot add operation to queue because queue is full. waiting for room to open up");
                Thread.Sleep(500);
				QueueOperation(operation);
				return;
            }
			_operationQueue.Enqueue(operation);
		}

		public IList<ThreadStart> GetQueueItems()
		{
			return _operationQueue.ToList();
		}

		public void StartOperationProcessingThread()
		{
			_logger.LogInformation("starting operation processing thread");
            StopOperationrocessingThread();
            ThreadStart threadDelegate = new ThreadStart(() => StartOperationProcessing());
            _processingThread = new Thread(threadDelegate);
            _processingThread.Start();
        }

		public void StopOperationrocessingThread()
		{
            if (_processingThread != null)
            {
                _logger.LogInformation("stopping operation processing thread");
                
            }
        }

		private void StartOperationProcessing()
		{
			var remainingMillis = GetRemainingMillis();
			if(remainingMillis <= 0)
			{
				if(GetSize() > 0)
				{
					_logger.LogInformation("processing next operation in the queue");
                    ProcessNextOperation();
                    StartOperationProcessing();
                }
				else
				{
					Thread.Sleep(500);
				}
				
			}
			else
			{
				Thread.Sleep(((int)remainingMillis));
			}
            StartOperationProcessing();
        }

		private void ProcessNextOperation()
		{
			if(GetSize() > 0)
			{
                ThreadStart operation = null;
                if (_operationQueue.TryDequeue(out operation))
                {
                    operation.Invoke();
                    _lastStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                }
                else
                {
                    ProcessNextOperation();
                }
            }
        }

		private long GetRemainingMillis()
		{
			var currentElapsedTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastStart;

			return _throttlePeriod - currentElapsedTime;
		}

		private int GetSize()
		{
			return _operationQueue.Count();
		}
	}
}

