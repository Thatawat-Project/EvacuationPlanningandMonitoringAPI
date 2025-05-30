using EvacuationPlanningandMonitoringAPI.Models;

namespace EvacuationPlanningandMonitoringAPI.Repositories.Interface
{
    public interface IRedisRepository
    {
        Task<List<RedisSyncData>> GetAsync(string key);
        Task SetAsync(RedisSyncData data,string key);
        Task DeleteAsync(string key);
    }
}
