using System.Threading.Channels;
using EvacuationPlanningandMonitoringAPI.Models;

namespace EvacuationPlanningandMonitoringAPI.Queue
{
    public class BackgroundSyncQueue
    {
        private readonly Channel<RedisSyncData> _queue = Channel.CreateUnbounded<RedisSyncData>();

        public void Queue(RedisSyncData data)
        {
            _queue.Writer.TryWrite(data);
        }

        public async Task<RedisSyncData?> DequeueAsync(CancellationToken token)
        {
            return await _queue.Reader.ReadAsync(token);
        }
    }
}
