using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Metadata
{
    internal static class MetadataRefreshScheduler
    {
        internal static TimeSpan minInterval = new TimeSpan(0, 1, 0);

        // Maximum delay supported by Task.Delay()
        private static readonly TimeSpan maxInterval = new TimeSpan(0, 0, 0, 0, int.MaxValue);

        internal static TimeSpan GetDelay(DateTime validUntil)
        {
            var timeRemaining = validUntil - DateTime.UtcNow;
            var delay = new TimeSpan(timeRemaining.Ticks / 2);

            if (delay < minInterval)
            {
                return minInterval;
            }

            if (delay > maxInterval)
            {
                return maxInterval;
            }

            return delay;
        }

        public static readonly TimeSpan DefaultMetadataCacheDuration = new TimeSpan(1, 0, 0);

        internal static DateTime CalculateMetadataValidUntil(this ICachedMetadata metadata)
        {
            return metadata.ValidUntil ??
                   DateTime.UtcNow.Add(metadata.CacheDuration ?? DefaultMetadataCacheDuration);
        }
    }
}
