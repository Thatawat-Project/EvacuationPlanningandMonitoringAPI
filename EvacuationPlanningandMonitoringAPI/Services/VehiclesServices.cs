using EvacuationPlanningandMonitoringAPI.Helpers;
using System.Text.Json;
using EvacuationPlanningandMonitoringAPI.Models.Db;
using EvacuationPlanningandMonitoringAPI.Models.DTOs;
using EvacuationPlanningandMonitoringAPI.Repositories.Interface;
using EvacuationPlanningandMonitoringAPI.Services.Interface;
using EvacuationPlanningandMonitoringAPI.Models;

namespace EvacuationPlanningandMonitoringAPI.Services
{
    public class VehiclesServices : IVehiclesServices
    {
        private readonly IVehiclesRepositories _repo;
        private readonly IRedisServices _redis;
        public VehiclesServices(IVehiclesRepositories repo, IRedisServices redis)
        {
            _repo = repo;
            _redis = redis;
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync() =>
            await _repo.GetAllAsync();

        public async Task<IEnumerable<Vehicle>> GetAllAsyncRedis()
        {
            var rawData = await _redis.GetAllAsync(GlobalConstants.RedisVehicleKey);
            var data = new List<Vehicle>();

            foreach (var item in rawData)
            {
                var vehicle = JsonSerializer.Deserialize<Vehicle>(item.Value);

                if (vehicle != null)
                {
                    var dto = new Vehicle
                    {
                        VehicleId = vehicle.VehicleId,
                        Capacity = vehicle.Capacity,
                        Type = vehicle.Type,
                        Speed = vehicle.Speed,
                        Status = vehicle.Status,
                        Latitude = vehicle.Latitude,
                        Longitude = vehicle.Longitude,
                        IsActive = vehicle.IsActive,
                        IsDelete = vehicle.IsDelete,
                        CreateBy = vehicle.CreateBy,
                        CreateDate = vehicle.CreateDate,
                        UpdateBy = vehicle.UpdateBy,
                        UpdateDate = vehicle.UpdateDate,
                    };
                    data.Add(dto);
                }
            }
            return data;
        }

        public async Task<Vehicle> GetByIdAsync(string id) =>
            await _repo.GetByIdAsync(id);

        public async Task<Vehicle?> GetByIdAsyncRedis(string vehicleId)
        {

            var rawData = await _redis.GetAllAsync(GlobalConstants.RedisVehicleKey);
            var sData = rawData.FirstOrDefault(x => x.Key == GlobalConstants.RedisVehicleKey + ":" + vehicleId);

            if(sData == null) return null;

            var data = new Vehicle();

            data = JsonSerializer.Deserialize<Vehicle>(sData.Value);

            return data;
        }

        public async Task CreateAsync(Vehicle data)
        {
            data.CreateDate = DateTime.Now;
            await _repo.AddAsync(data);
        }

        public async Task UpdateRedisToDbAsync(Vehicle data)
        {
            await _repo.UpdateRedisToDbAsync(data);
        }

        public async Task UpdateRedisAsync(string id,Vehicle data)
        {
            var rawData = await GetByIdAsyncRedis(id);
            if(rawData != null)
            {
                rawData.Longitude = data.Longitude;
                rawData.Latitude = data.Latitude;
                rawData.UpdateDate = data.UpdateDate;
            }

            var jsonData = JsonSerializer.Serialize(rawData);

            var redisData = new RedisSyncData()
            {
                Key = id,
                Value = jsonData,
                UpdateTime = data.UpdateDate
            };

            await _redis.CreateAsync(redisData,GlobalConstants.RedisVehicleKey);
        }

        public async Task DeleteAllAsync()
        {
            await _repo.DeleteAllAsync();
            await _redis.DeleteAsync(GlobalConstants.RedisVehicleKey);
        }
    }
}
