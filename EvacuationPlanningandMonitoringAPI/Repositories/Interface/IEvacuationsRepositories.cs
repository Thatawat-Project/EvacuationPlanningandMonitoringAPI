using EvacuationPlanningandMonitoringAPI.Models.Db;

namespace EvacuationPlanningandMonitoringAPI.Repositories.Interface
{
    public interface IEvacuationsRepositories
    {
        Task<IEnumerable<EvacuationsPlan>> GetAllPlanAsync();
        Task<EvacuationsPlan> GetByPlanIdAsync(int id);
        Task<IEnumerable<EvacuationLog>> GetAllLogAsync();
        Task<int> CreatePlanAsync(EvacuationsPlan data);
        Task CreateEvacuationsLog(EvacuationLog data);
        Task UpdatePlanRedisToDbAsync(EvacuationsPlan data);
        Task<EvacuationLog> GetLogAsync(EvacuationLog data);
        Task DeletePlanAsync();
    }
}
