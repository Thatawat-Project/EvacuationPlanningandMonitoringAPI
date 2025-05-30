using EvacuationPlanningandMonitoringAPI.Models.Db;
using EvacuationPlanningandMonitoringAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EvacuationPlanningandMonitoringAPI.Repositories
{
    public class EvacuationsRepositories : IEvacuationsRepositories
    {
        private readonly EvacuationPlanningandMonitoringDbContext _context;
        public EvacuationsRepositories(EvacuationPlanningandMonitoringDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<EvacuationsPlan>> GetAllPlanAsync() =>
            await _context.EvacuationsPlans.ToListAsync();

        public async Task<EvacuationsPlan> GetByPlanIdAsync(int id) =>
           await _context.EvacuationsPlans.FindAsync(id);

        public async Task<int> CreatePlanAsync(EvacuationsPlan data)
        {
            _context.EvacuationsPlans.Add(data);
            await _context.SaveChangesAsync();
            return data.Id;
        }

        public async Task<IEnumerable<EvacuationLog>> GetAllLogAsync() =>
         await _context.EvacuationLogs.ToListAsync();

        public async Task<EvacuationLog?> GetLogAsync(EvacuationLog data)
        {
            return await _context.EvacuationLogs.Where(x=>x.ZoneId == data.ZoneId && x.VehicleId == data.VehicleId && x.CreateDate == data.CreateDate).FirstOrDefaultAsync();
        }

        public async Task CreateEvacuationsLog(EvacuationLog data)
        {
            _context.EvacuationLogs.Add(data);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePlanRedisToDbAsync(EvacuationsPlan data)
        {
            var rawData = await _context.EvacuationsPlans.Where(x => x.ZoneId == data.ZoneId && x.VehicleId == data.VehicleId).OrderBy(x=>x.CreateDate).FirstOrDefaultAsync();
            if (rawData == null) return;
            
            rawData.Status = data.Status;
            rawData.Eta = data.Eta;
            rawData.UpdateDate = data.UpdateDate;

            await _context.SaveChangesAsync();
        }

        public async Task DeletePlanAsync()
        {      
            var rawData = await _context.EvacuationsPlans.ToListAsync();
            foreach (var item in rawData)
            {
                item.IsDelete = true;
                item.UpdateDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }
    }
}
