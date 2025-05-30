using EvacuationPlanningandMonitoringAPI.Models;

namespace EvacuationPlanningandMonitoringAPI.Queue.Interface
{
    public interface IBackgroundSyncQueue
    {
        void Queue(RedisSyncData data);
        Task<RedisSyncData?> DequeueAsync(CancellationToken token);
    }
}
