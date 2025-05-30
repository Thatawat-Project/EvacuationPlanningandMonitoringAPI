using EvacuationPlanningandMonitoringAPI.Models.Db;

namespace EvacuationPlanningandMonitoringAPI.Repositories.Interface
{
    public interface IVehiclesRepositories
    {
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<Vehicle> GetByIdAsync(string id);
        Task AddAsync(Vehicle data);
        Task UpdateRedisToDbAsync(Vehicle data);
        Task DeleteAllAsync();
    }
}
