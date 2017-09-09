using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public class RateLimiter
    {
        /// <summary>
        /// Queue of times when database requests were made
        /// </summary>
        private Queue<DateTime> requestTimes = new Queue<DateTime>();

        public bool Enabled { get; set; }

        public int MaxRequests { get; set; }

        public int RequestLimitTime { get; set; }

        public RateLimiter(bool en, int maxRequests, int limitTime)
        {
            this.Enabled = en;
            this.MaxRequests = maxRequests;
            this.RequestLimitTime = limitTime;
        }

        public void DoWait()
        {
            if (!this.Enabled)
                return;

            // Check rate of requests - maximum is MaxRequests requests every RequestLimitTime milliseconds
            DateTime oldest;
            lock (requestTimes)
            {
                while (requestTimes.Count >= this.MaxRequests)
                {
                    oldest = requestTimes.Peek();
                    double requestAge = (DateTime.Now - oldest).TotalMilliseconds;
                    if (requestAge > this.RequestLimitTime)
                        requestTimes.Dequeue();
                    else
                        Thread.Sleep((RequestLimitTime + 1 - (int)requestAge));

                }
                requestTimes.Enqueue(DateTime.Now);
            }
        }
    }
}
