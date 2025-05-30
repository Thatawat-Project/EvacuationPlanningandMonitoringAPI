using EvacuationPlanningandMonitoringAPI.Repositories.Interface;
using EvacuationPlanningandMonitoringAPI.Services.Interface;
using EvacuationPlanningandMonitoringAPI.Models.Db;
using EvacuationPlanningandMonitoringAPI.Helpers;
using EvacuationPlanningandMonitoringAPI.Models;
using System.Text.Json;
using EvacuationPlanningandMonitoringAPI.Models.DTOs;

namespace EvacuationPlanningandMonitoringAPI.Services
{
    public class EvacuationsServices : IEvacuationsServices
    {
        private readonly IEvacuationsRepositories _repo;
        private readonly IRedisServices _redis;
        public EvacuationsServices(IEvacuationsRepositories repo, IRedisServices redis)
        {
            _repo = repo;
            _redis = redis;
        }

        public async Task<IEnumerable<EvacuationsPlan>> GetAllPlanAsync()=>
            await _repo.GetAllPlanAsync();

        public async Task<EvacuationsPlan> GetPlanByIdAsync(int id) =>
            await _repo.GetByPlanIdAsync(id);

        public async Task<int> CreatePlanAsync(EvacuationsPlan data)
        {
            return await _repo.CreatePlanAsync(data);
        }

        public async Task UpdatePlanRedisToDbAsync(EvacuationsPlan data)
        {
            await _repo.UpdatePlanRedisToDbAsync(data);
        }

        public async Task CreateEvacuationsLog(EvacuationLog data)
        {
            await _repo.CreateEvacuationsLog(data);
        }

        public async Task<IEnumerable<EvacuationLog>> GetAllLogAsyncRedis()
        {
            var rawData = await _redis.GetAllAsync(GlobalConstants.RedisEvacuationsLog);
            var data = new List<EvacuationLog>();

            foreach (var item in rawData)
            {
                var deJsonData = JsonSerializer.Deserialize<EvacuationLog>(item.Value);

                if (deJsonData != null)
                {
                    var dto = new EvacuationLog
                    {
                        ZoneId = deJsonData.ZoneId,
                        VehicleId = deJsonData.VehicleId,
                        EvacuatedCount = deJsonData.EvacuatedCount,
                        EvacuationTime = deJsonData.EvacuationTime,
                        Status = deJsonData.Status,
                        StartTime = deJsonData.StartTime,
                        EndTime = deJsonData.EndTime,
                        Eta = deJsonData.Eta,
                        CreateDate = deJsonData.CreateDate
                    };
                    data.Add(dto);
                }
            }
            return data;
        }

        public async Task<IEnumerable<EvacuationsPlan>> GetAllPlanAsyncRedis()
        {
            var rawData = await _redis.GetAllAsync(GlobalConstants.RedisEvacuationsPlan);
            var data = new List<EvacuationsPlan>();

            foreach (var item in rawData)
            {
                var deJsonData = JsonSerializer.Deserialize<EvacuationsPlan>(item.Value);

                if (deJsonData != null)
                {
                    var temData = new EvacuationsPlan
                    {
                        Id = deJsonData.Id,
                        ZoneId = deJsonData.ZoneId,
                        VehicleId = deJsonData.VehicleId,
                        Eta = deJsonData.Eta,
                        EvacuationTarget = deJsonData.EvacuationTarget,
                        EvacuatedCount = deJsonData.EvacuatedCount,
                        TripNumber = deJsonData.TripNumber,
                        Status = deJsonData.Status,
                        CreateDate = deJsonData.CreateDate,
                        UpdateDate = deJsonData.UpdateDate
                    };
                    data.Add(temData);
                }
            }
            return data;
        }

        public async Task<EvacuationsPlan> GetPlanByIdAsyncRedis(string id)
        {
            var rawData = await _redis.GetAllAsync(GlobalConstants.RedisEvacuationsPlan);

            var sData = rawData.FirstOrDefault(x => x.Key == id);

            var data = new EvacuationsPlan();

            if (sData != null)
                data = JsonSerializer.Deserialize<EvacuationsPlan>(sData.Value);

            return data;
        }

        public async Task UpdatePlanRedisAsync(string id, EvacuationsPlan data)
        {
            var rawData = await GetPlanByIdAsyncRedis(id);
            if (rawData != null)
            {
                rawData.Status = data.Status;
                rawData.Eta = data.Eta;
                rawData.UpdateDate = data.UpdateDate;
            }

            var jsonData = JsonSerializer.Serialize(rawData);

            var redisData = new RedisSyncData()
            {
                Key = id,
                Value = jsonData,
                UpdateTime = data.UpdateDate
            };

            await _redis.CreateAsync(redisData, GlobalConstants.RedisEvacuationsPlan);
        }

        public async Task<IEnumerable<EvacuationLog>> GetAllLogAsync()
        {
           return await _repo.GetAllLogAsync();
        }

        public async Task<EvacuationLog> GetLogAsync(EvacuationLog data)
        {
            return await _repo.GetLogAsync(data);
        }

        public async Task DeletePlanAsync()
        {
            await _repo.DeletePlanAsync();
            await _redis.DeleteAsync(GlobalConstants.RedisEvacuationsPlan);
        }
    }
}
