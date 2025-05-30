using System;
using EvacuationPlanningandMonitoringAPI.Models.Db;
using EvacuationPlanningandMonitoringAPI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EvacuationPlanningandMonitoringAPI.Repositories
{
    public class EvacuationZoneRepository : IEvacuationZonesRepositories
    {
        private readonly EvacuationPlanningandMonitoringDbContext _context;
        public EvacuationZoneRepository(EvacuationPlanningandMonitoringDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EvacuationZone>> GetAllAsync() =>
            await _context.EvacuationZones.ToListAsync();

        public async Task<EvacuationZone> GetByIdAsync(string id) =>
            await _context.EvacuationZones.FindAsync(id);

        public async Task AddAsync(EvacuationZone data)
        {         
            _context.EvacuationZones.Add(data);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EvacuationZone data)
        {
            var rawData = await _context.EvacuationZones.FindAsync(data.ZoneId);
            if (rawData == null) return;

            rawData.Status = data.Status;
            rawData.UpdateDate = data.UpdateDate;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteZoneAsync()
        {
            var rawData = await _context.EvacuationZones.ToListAsync();
            foreach (var item in rawData)
            {
                item.IsDelete = true;
                item.UpdateDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }
    }
}
