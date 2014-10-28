using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    class MetadataRefreshScheduler
    {
        internal static TimeSpan minInternval = new TimeSpan(0, 1, 0);
        
        // Maximum delay supported by Task.Delay()
        private static readonly TimeSpan maxInterval = new TimeSpan(0, 0, 0, 0, int.MaxValue);

        internal static TimeSpan GetDelay(DateTime validUntil)
        {
            var timeRemaining = validUntil - DateTime.UtcNow;
            var delay = new TimeSpan(timeRemaining.Ticks / 2);

            if(delay < minInternval)
            {
                return minInternval;
            }

            if(delay > maxInterval)
            {
                return maxInterval;
            }

            return delay;
        }
    }
}
