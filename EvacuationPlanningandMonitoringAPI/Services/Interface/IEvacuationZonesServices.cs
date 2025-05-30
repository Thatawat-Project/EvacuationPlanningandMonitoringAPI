using EvacuationPlanningandMonitoringAPI.Models.Db;
using EvacuationPlanningandMonitoringAPI.Models.DTOs;

namespace EvacuationPlanningandMonitoringAPI.Services.Interface
{
    public interface IEvacuationZonesServices
    {
        Task<IEnumerable<EvacuationZone>> GetAllAsync();
        Task<IEnumerable<EvacuationZone>> GetAllAsyncRedis();
        Task<EvacuationZone> GetByIdAsync(string id);
        Task<EvacuationZone> GetByIdAsyncRedis(string id);
        Task CreateAsync(EvacuationZone data);
        Task UpdateAsync(EvacuationZone data);
        Task DeleteZoneAsync();
        Task UpdateRedisAsync(string id, EvacuationZone data);
    }
}
