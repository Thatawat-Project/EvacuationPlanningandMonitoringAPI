using EvacuationPlanningandMonitoringAPI.Models.Db;

namespace EvacuationPlanningandMonitoringAPI.Repositories.Interface
{
    public interface IEvacuationZonesRepositories
    {
        Task<IEnumerable<EvacuationZone>> GetAllAsync();
        Task<EvacuationZone> GetByIdAsync(string id);
        Task AddAsync(EvacuationZone data);
        Task UpdateAsync(EvacuationZone data);
        Task DeleteZoneAsync();
    }
}
