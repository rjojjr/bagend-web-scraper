using System;
namespace bagend_web_scraper.StockMarket.Operations
{
	public interface OperationProcessor
	{
        void QueueOperation(ThreadStart operation);

        void StartOperationProcessingThread();

        void StopOperationrocessingThread();
    }
}

