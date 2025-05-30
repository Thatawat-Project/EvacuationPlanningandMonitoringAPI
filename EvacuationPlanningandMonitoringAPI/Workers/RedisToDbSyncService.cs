using EvacuationPlanningandMonitoringAPI.Helpers;
using EvacuationPlanningandMonitoringAPI.Models;
using EvacuationPlanningandMonitoringAPI.Models.Db;
using EvacuationPlanningandMonitoringAPI.Models.DTOs;
using EvacuationPlanningandMonitoringAPI.Services.Interface;
using System.Text.Json;

public class RedisToDbSyncService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

    public RedisToDbSyncService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();

            var vehiclesServices = scope.ServiceProvider.GetRequiredService<IVehiclesServices>();
            var redisServices = scope.ServiceProvider.GetRequiredService<IRedisServices>();
            var evacuationsServices = scope.ServiceProvider.GetRequiredService<IEvacuationsServices>();
            var evacuationsZoneService = scope.ServiceProvider.GetRequiredService<IEvacuationZonesServices>();

            var rawRedisVehiclesData = await vehiclesServices.GetAllAsyncRedis();
            var rawRedisEvacuationsLogData = await evacuationsServices.GetAllLogAsyncRedis();
            var rawRedisEvacuationPlanData = await evacuationsServices.GetAllPlanAsyncRedis();
            var rawRedisEvacuationZoneData = await evacuationsZoneService.GetAllAsyncRedis();

            if (rawRedisVehiclesData.Any())
            {
                foreach (var item in rawRedisVehiclesData)
                {
                    var rawData = await vehiclesServices.GetByIdAsync(item.VehicleId);
                    if(rawData != null)
                    {
                        if (item.UpdateDate > rawData.UpdateDate)
                        {
                            await vehiclesServices.UpdateRedisToDbAsync(item);
                        }
                    }
                }
            }
            else
            {
                var rawVehiclesData = await vehiclesServices.GetAllAsync();
                rawVehiclesData = rawVehiclesData.Where(x => x.IsDelete != true).ToList();
                if (rawVehiclesData.Any())
                {
                    foreach (var item in rawVehiclesData)
                    {
                        var jsonData = JsonSerializer.Serialize(item);

                        var redisData = new RedisSyncData()
                        {
                            Key = GlobalConstants.RedisVehicleKey + ":" + item.VehicleId,
                            Value = jsonData,
                            UpdateTime = item.UpdateDate,
                        };
                        await redisServices.CreateAsync(redisData, GlobalConstants.RedisVehicleKey);
                    }
                }
            }

            if (rawRedisEvacuationsLogData.Any())
            {
                foreach (var item in rawRedisEvacuationsLogData)
                {
                    var data = new EvacuationLog
                    {
                        ZoneId = item.ZoneId,
                        VehicleId = item.VehicleId,
                        EvacuatedCount = item.EvacuatedCount,
                        EvacuationTime = item.EvacuationTime,
                        Status = item.Status,
                        Eta = item.Eta,
                        StartTime = item.StartTime,
                        EndTime = item.EndTime,
                        CreateDate = item.CreateDate
                    };
                    
                    await evacuationsServices.CreateEvacuationsLog(data);
                    await redisServices.DeleteAsync(GlobalConstants.RedisEvacuationsLog + ":" + item.ZoneId + ":" + item.VehicleId + ":" + item.CreateDate?.ToString("yyyyMMddHHmmss"));
                }
            }

            if (rawRedisEvacuationPlanData.Any())
            {
                foreach (var item in rawRedisEvacuationPlanData)
                {
                    var rawData = await evacuationsServices.GetPlanByIdAsync(item.Id);
                    if(rawData != null)
                    {
                        if (item.UpdateDate > rawData.UpdateDate)
                        {
                            await evacuationsServices.UpdatePlanRedisToDbAsync(item);
                        }
                    }
                }
            }
            else
            {
                var rawData = await evacuationsServices.GetAllPlanAsync();
                rawData = rawData.Where(x => x.IsDelete != true);
                if (rawData.Any())
                {
                    foreach (var item in rawData)
                    {
                        var jsonData = JsonSerializer.Serialize(item);

                        var redisData = new RedisSyncData()
                        {
                            Key = GlobalConstants.RedisEvacuationsPlan + ":" + item.ZoneId + ":" + item.VehicleId,
                            Value = jsonData,
                            UpdateTime = item.UpdateDate,
                        };
                        await redisServices.CreateAsync(redisData, GlobalConstants.RedisEvacuationsPlan);
                    }
                }
            }

            if (rawRedisEvacuationZoneData.Any())
            {
                foreach (var item in rawRedisEvacuationZoneData)
                {
                    var rawData = await evacuationsZoneService.GetByIdAsync(item.ZoneId);
                    if(rawData != null)
                    {
                        if(item.UpdateDate > rawData.UpdateDate)
                        {
                            var data = new EvacuationZone
                            {
                                ZoneId = item.ZoneId,
                                Status = item.Status,
                                UpdateDate = item.UpdateDate,
                            };
                            await evacuationsZoneService.UpdateAsync(data);
                        }
                    }
                }
            }
            else
            {
                var rawData = await evacuationsZoneService.GetAllAsync();
                rawData = rawData.Where(x => x.IsDelete != true);
                foreach (var item in rawData)
                {
                    var jsonData = JsonSerializer.Serialize(item);

                    var redisData = new RedisSyncData()
                    {
                        Key = GlobalConstants.RedisEvacuationsZone + ":" + item.ZoneId,
                        Value = jsonData,
                        UpdateTime = item.UpdateDate,
                    };
                    await redisServices.CreateAsync(redisData, GlobalConstants.RedisEvacuationsZone);
                }
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}
