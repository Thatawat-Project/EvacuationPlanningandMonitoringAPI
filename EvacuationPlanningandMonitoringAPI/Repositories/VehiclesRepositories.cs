using EvacuationPlanningandMonitoringAPI.Models.Db;
using EvacuationPlanningandMonitoringAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EvacuationPlanningandMonitoringAPI.Repositories
{
    public class VehiclesRepositories : IVehiclesRepositories
    {
        private readonly EvacuationPlanningandMonitoringDbContext _context;
        public VehiclesRepositories(EvacuationPlanningandMonitoringDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync() =>
            await _context.Vehicles.ToListAsync();

        public async Task<Vehicle> GetByIdAsync(string id) => 
            await _context.Vehicles.FindAsync(id);

        public async Task AddAsync(Vehicle data)
        {
            _context.Vehicles.Add(data);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRedisToDbAsync(Vehicle data) 
        {
            var vehiclesData = await _context.Vehicles.FindAsync(data.VehicleId);
            if (vehiclesData == null) return;

            vehiclesData.Longitude = data.Longitude;
            vehiclesData.Latitude = data.Latitude;
            vehiclesData.UpdateDate = data.UpdateDate;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAllAsync()
        {
            var rawData = await _context.Vehicles.ToListAsync();
            foreach (var item in rawData)
            {
                item.IsDelete = true;
                item.UpdateDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

    }
}
