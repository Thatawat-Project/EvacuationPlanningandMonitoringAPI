using EvacuationPlanningandMonitoringAPI.Models;
using EvacuationPlanningandMonitoringAPI.Models.Db;
using EvacuationPlanningandMonitoringAPI.Models.DTOs;

namespace EvacuationPlanningandMonitoringAPI.Services.Interface
{
    public interface IVehiclesServices
    {
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<IEnumerable<Vehicle>> GetAllAsyncRedis();
        Task<Vehicle> GetByIdAsync(string id);
        Task<Vehicle?> GetByIdAsyncRedis(string id);
        Task CreateAsync(Vehicle data);
        Task UpdateRedisToDbAsync(Vehicle data);
        Task UpdateRedisAsync(string Id, Vehicle data);
        Task DeleteAllAsync();
    }
}
