using EvacuationPlanningandMonitoringAPI.Models;
using EvacuationPlanningandMonitoringAPI.Repositories.Interface;
using EvacuationPlanningandMonitoringAPI.Services.Interface;

namespace EvacuationPlanningandMonitoringAPI.Services
{
    public class RedisServices : IRedisServices
    {
        private readonly IRedisRepository _repo;

        public RedisServices(IRedisRepository repo) 
        { 
            _repo = repo; 
        }

        public async Task<List<RedisSyncData>> GetAllAsync(string key)
        {
            return await _repo.GetAsync(key);
        }

        public async Task CreateAsync(RedisSyncData data,string key)
        {
            await _repo.SetAsync(data,key);
        }

        public async Task DeleteAsync(string key)
        {
            await _repo.DeleteAsync(key);
        }
    }
}
