using System;
using System.Collections.Concurrent;
using bagend_web_scraper.Config;
using Microsoft.Extensions.Options;

namespace bagend_web_scraper.StockMarket.Operations
{
	public class ThrottledFIFOOperationProcessor : OperationProcessor
	{

		private const int WaitForSpaceInQueuePeriod = 50;

		private long _lastStart { get; set; } = 0;
		private long _throttlePeriod { get; set; } = 0;
		private int _maxQueueSize { get; set; } = 0;
		private int _maxThreads { get; set; } = 1;
		private  Thread _processingThread { get; set; } = null!;
		private ConcurrentQueue<ThreadStart> _operationQueue;
		private readonly ILogger<ThrottledFIFOOperationProcessor> _logger;

        public ThrottledFIFOOperationProcessor(IOptions<PolygonApiConfig> polygonApiConfig)
		{
			_throttlePeriod = polygonApiConfig.Value.ThrottleMilliseconds;
			_maxQueueSize = polygonApiConfig.Value.MaxQueueLength;
			var threads = polygonApiConfig.Value.MaxThreads;
			_maxThreads = threads > 0 ? threads : 1;
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
				_logger.LogDebug("cannot add operation to queue because queue is full. waiting for room to open up");
                Thread.Sleep(WaitForSpaceInQueuePeriod);
				QueueOperation(operation);
				return;
            }
			_operationQueue.Enqueue(operation);
		}

		public IList<ThreadStart> GetQueueItems()
		{
			return _operationQueue.ToList();
		}

		public int GetQueueSize()
		{
			return _operationQueue.Count();
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

		private ThreadStart threadFactory(Action action, Action callback)
		{
			var job = () =>
			{
				action.Invoke();
				callback.Invoke();
			};

			return new ThreadStart(job);
        }

		private void executeThread(Action action, Action callback)
		{
			new Thread(threadFactory(action, callback)).Start();
		}

		private void StartOperationProcessing()
		{
			Action callback = null;
			callback = () => executeThread(() => {
                _logger.LogDebug("processing next job in the queue");
                ProcessNextOperation();
            }, callback);



            var remainingMillis = GetRemainingMillis();

			var threads = new ThreadStart[_maxThreads];
			foreach(ThreadStart ts in threads)
			{
				callback.Invoke();
			}
        }

		private void ProcessNextOperation()
		{

			var process = () =>
			{
				ThreadStart operation = null;
				if (_operationQueue.TryDequeue(out operation))
                {
                    operation.Invoke();
                }
				else
                {
					_logger.LogWarning("failed to fetch operation from queue");
                    Thread.Sleep(50);
                    ProcessNextOperation();
				}
			};

			if(GetSize() > 0)
			{
				if(GetRemainingMillis() <= 0)
                {
                    lock (this)
                    {
                        _lastStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    }
                    process.Invoke();
				}
				else
				{
					Thread.Sleep(WaitForSpaceInQueuePeriod);
					ProcessNextOperation();
				}

            }
            else
            {
                Thread.Sleep(100);
                ProcessNextOperation();
            }
        }

		private long GetRemainingMillis()
		{
			if(_throttlePeriod > 0)
			{
                long currentElapsedTime = 0;

                lock (this)
                {
                    currentElapsedTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _lastStart;
                }

                return _throttlePeriod - currentElapsedTime;
            }
			return 0;
		}

		private int GetSize()
		{
			return _operationQueue.Count();
		}
	}
}

