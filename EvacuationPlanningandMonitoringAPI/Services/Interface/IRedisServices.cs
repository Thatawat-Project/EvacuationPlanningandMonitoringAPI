using EvacuationPlanningandMonitoringAPI.Models;

namespace EvacuationPlanningandMonitoringAPI.Services.Interface
{
    public interface IRedisServices
    {
        Task<List<RedisSyncData>> GetAllAsync(string key);
        Task CreateAsync(RedisSyncData data,string key);
        Task DeleteAsync(string key);
    }
}
