using EvacuationPlanningandMonitoringAPI.Models.Db;
using EvacuationPlanningandMonitoringAPI.Models.DTOs;

namespace EvacuationPlanningandMonitoringAPI.Services.Interface
{
    public interface IEvacuationsServices
    {
        Task<IEnumerable<EvacuationsPlan>> GetAllPlanAsync();
        Task<EvacuationsPlan> GetPlanByIdAsync(int id);
        Task<EvacuationsPlan> GetPlanByIdAsyncRedis(string id);
        Task<IEnumerable<EvacuationLog>> GetAllLogAsync();
        Task<IEnumerable<EvacuationLog>> GetAllLogAsyncRedis();
        Task<int> CreatePlanAsync(EvacuationsPlan data);
        Task CreateEvacuationsLog(EvacuationLog data);
        Task UpdatePlanRedisToDbAsync(EvacuationsPlan data);
        Task<IEnumerable<EvacuationsPlan>> GetAllPlanAsyncRedis();
        Task UpdatePlanRedisAsync(string id ,EvacuationsPlan data);
        Task<EvacuationLog> GetLogAsync(EvacuationLog data);
        Task DeletePlanAsync();
    }
}
