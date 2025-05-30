using EvacuationPlanningandMonitoringAPI.Helpers;
using System.Text.Json;
using EvacuationPlanningandMonitoringAPI.Models.Db;
using EvacuationPlanningandMonitoringAPI.Repositories.Interface;
using EvacuationPlanningandMonitoringAPI.Services.Interface;
using StackExchange.Redis;
using EvacuationPlanningandMonitoringAPI.Models.DTOs;
using EvacuationPlanningandMonitoringAPI.Models;

namespace EvacuationPlanningandMonitoringAPI.Services
{
    public class EvacuationZonesServices : IEvacuationZonesServices
    {
        private readonly IEvacuationZonesRepositories _repo;
        private readonly IRedisServices _redis;
        public EvacuationZonesServices(IEvacuationZonesRepositories repo, IRedisServices redis)
        {
            _repo = repo;
            _redis = redis;
        }

        public async Task<IEnumerable<EvacuationZone>> GetAllAsyncRedis()
        {
            var rawData = await _redis.GetAllAsync(GlobalConstants.RedisEvacuationsZone);
            var data = new List<EvacuationZone>();

            foreach (var item in rawData)
            {
                var deJsonData = JsonSerializer.Deserialize<EvacuationZone>(item.Value);

                if (deJsonData != null)
                {
                    var dto = new EvacuationZone
                    {
                        ZoneId = deJsonData.ZoneId,
                        NameZone = deJsonData.NameZone,
                        NumberPeople = deJsonData.NumberPeople,
                        UrgencyLevel = deJsonData.UrgencyLevel,
                        Latitude = deJsonData.Latitude,
                        Longitude = deJsonData.Longitude,
                        Status = deJsonData.Status,
                        IsActive = deJsonData.IsActive,
                        IsDelete = deJsonData.IsDelete,
                        CreateBy = deJsonData.CreateBy,
                        CreateDate = deJsonData.CreateDate,
                        UpdateBy = deJsonData.UpdateBy,
                        UpdateDate = deJsonData.UpdateDate,
                    };
                    data.Add(dto);
                }
            }
            return data;
        }

        public async Task<IEnumerable<EvacuationZone>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task CreateAsync(EvacuationZone data)
        {
            data.CreateDate = DateTime.Now;
            await _repo.AddAsync(data);
        }

        public async Task UpdateAsync(EvacuationZone data)
        {
            await _repo.UpdateAsync(data);
        }

        public async Task<EvacuationZone?> GetByIdAsync(string id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<EvacuationZone?> GetByIdAsyncRedis(string id)
        {
            var rawData = await _redis.GetAllAsync(GlobalConstants.RedisEvacuationsZone);

            var sData = rawData.FirstOrDefault(x => x.Key == GlobalConstants.RedisEvacuationsZone + ":" + id);

            if (sData == null) return null;

            var data = new EvacuationZone();

            data = JsonSerializer.Deserialize<EvacuationZone>(sData.Value);

            return data;
        }

        public async Task DeleteZoneAsync()
        {
            await _repo.DeleteZoneAsync();
            await _redis.DeleteAsync(GlobalConstants.RedisEvacuationsZone);
        }

        public async Task UpdateRedisAsync(string id, EvacuationZone data)
        {
            var rawData = await GetByIdAsyncRedis(id);
            if (rawData != null)
            {
                rawData.Status = data.Status?.ToString();
                rawData.UpdateDate = data.UpdateDate;
            }

            var jsonData = JsonSerializer.Serialize(rawData);

            var redisData = new RedisSyncData()
            {
                Key = id,
                Value = jsonData,
                UpdateTime = data.UpdateDate
            };

            await _redis.CreateAsync(redisData, GlobalConstants.RedisVehicleKey);
        }
    }
}
