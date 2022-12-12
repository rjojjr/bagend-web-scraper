using System;
using System.Runtime.CompilerServices;

namespace bagend_web_scraper.Threads
{
	public class ThreadTracker
	{

        private bool _isThreadActive { get; set; }
        private readonly object threadLock;

        public ThreadTracker()
        {
            threadLock = new object();
            _isThreadActive = false;
            
        }

        public bool IsThreadActive()
        {
            lock (this.threadLock)
            {
                return _isThreadActive;
            }
        }

        public void ActivateThread()
        {
            lock (this.threadLock)
            {
                _isThreadActive = true;
            }
        }

        public void DeactivateThread()
        {
            lock (this.threadLock)
            {
                _isThreadActive = false;
            }
        }
    }
}

