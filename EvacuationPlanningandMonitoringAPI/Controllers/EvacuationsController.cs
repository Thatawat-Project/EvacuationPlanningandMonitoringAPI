using EvacuationPlanningandMonitoringAPI.Models.DTOs;
using EvacuationPlanningandMonitoringAPI.Services.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using EvacuationPlanningandMonitoringAPI.Models;
using EvacuationPlanningandMonitoringAPI.Models.Db;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using EvacuationPlanningandMonitoringAPI.Helpers;
using System.Numerics;
using EvacuationPlanningandMonitoringAPI.Services;

namespace EvacuationPlanningandMonitoringAPI.Controllers.Api
{
    [ApiController]
    public class EvacuationsController : ControllerBase
    {
        private readonly IEvacuationsServices _EvacuationsServices;
        private readonly IEvacuationZonesServices _EvacuationZonesServices;
        private readonly IRedisServices _RedisServices;
        private readonly IVehiclesServices _VehiclesServices;
        public EvacuationsController(IEvacuationsServices EvacuationsServices, IEvacuationZonesServices EvacuationZonesServices, IRedisServices redisServices, IVehiclesServices vehiclesServices)
        {
            _EvacuationsServices = EvacuationsServices;
            _EvacuationZonesServices = EvacuationZonesServices;
            _RedisServices = redisServices;
            _VehiclesServices = vehiclesServices;
        }


        [HttpGet]
        [Route("api/evacuations/plan")]
        public async Task<IActionResult> GetAllPlanData()
        {
            var rawData = await _EvacuationsServices.GetAllPlanAsyncRedis();
            var planData = rawData.Where(x => x.IsDelete != true).OrderBy(x => x.CreateDate).ToList();
            var data = new List<DisplayEvacuationPlan>();
            foreach (var item in planData)
            {
                var ETA = Calculate.ConvertTimeStringToReadable(item.Eta);
                var displayData = new DisplayEvacuationPlan()
                {
                    ZoneId = item.ZoneId,
                    VehicleId = item.VehicleId,
                    EvacuationTarget = item.EvacuationTarget,
                    ETA = ETA,
                    NumberOfPeople = item.EvacuatedCount,
                    TripNumber = item.TripNumber,
                    Status = item.Status,
                    CreateDate = item.CreateDate,
                    UpdateDate = item.UpdateDate
                };

                data.Add(displayData);
            }

            return Ok(data);
        }

        [HttpPost]
        [Route("api/evacuations/plan")]
        public async Task<IActionResult> CreatePlan([FromBody] EvacuationsPlanDto Dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            DateTime dateTime = DateTime.Now;

            var VehiclesData = await _VehiclesServices.GetByIdAsyncRedis(GlobalConstants.RedisVehicleKey + ":" + Dto.VehicleId);
            var EvacuationZoneData = await _EvacuationZonesServices.GetByIdAsyncRedis(GlobalConstants.RedisEvacuationsZone + ":" + Dto.ZoneId);

            if (VehiclesData == null) return NotFound($"Vehicle with ID {Dto.VehicleId} not found.");
            if (EvacuationZoneData == null) return NotFound($"Evacuation Zone with ID {Dto.ZoneId} not found.");

            var ETA = Calculate.CalculateETA(EvacuationZoneData.Latitude, EvacuationZoneData.Longitude, VehiclesData.Latitude, VehiclesData.Longitude, VehiclesData.Speed);
            string stringETA = Calculate.GetRemainingTimeString(ETA);
            var data = new EvacuationsPlan
            {
                ZoneId = Dto.ZoneId,
                VehicleId = Dto.VehicleId,
                Eta = stringETA,
                EvacuationTarget = Dto.EvacuationTarget,
                EvacuatedCount = Dto.EvacuatedCount,
                TripNumber = Dto.TripNumber,
                Status = Dto.Status.ToString(),
                CreateDate = dateTime,
                UpdateDate = dateTime
            };

            var Id = await _EvacuationsServices.CreatePlanAsync(data);

            data.Id = Id;

            var jsonData = JsonSerializer.Serialize(data);

            var redisData = new RedisSyncData
            {
                Key = GlobalConstants.RedisEvacuationsPlan + ":" + data.ZoneId + ":" + data.VehicleId + ":" + data.Id.ToString(),
                Value = jsonData,
                UpdateTime = dateTime
            };

            await _RedisServices.CreateAsync(redisData, GlobalConstants.RedisEvacuationsPlan);

            return Ok(data);
        }

        [HttpPut]
        [Route("api/evacuations/update")]
        public async Task<IActionResult> Update([FromBody] EvacuationLogDto Dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dateTime = DateTime.Now;

            var VehiclesKey = "";
            var EvacuationZoneKey = "";

            var statusPriority = new List<string>
            {
                "ZONE_EVACUATING",
                "ZONE_FAILED",
                "ZONE_PENDING",
                "ZONE_CANCELED",
                "ZONE_COMPLETED"
            };

            var VehiclesData = await _VehiclesServices.GetByIdAsyncRedis(GlobalConstants.RedisVehicleKey + ":" + Dto.VehicleId);
            var EvacuationZoneData = await _EvacuationZonesServices.GetByIdAsyncRedis(GlobalConstants.RedisEvacuationsZone + ":" + Dto.ZoneId);
            var EvacuationPlanData = await _EvacuationsServices.GetPlanByIdAsyncRedis(GlobalConstants.RedisEvacuationsPlan + ":" + Dto.ZoneId + ":" + Dto.VehicleId);

            if(VehiclesData == null) return NotFound($"Vehicle with ID {Dto.VehicleId} not found.");
            if (EvacuationZoneData == null) return NotFound($"Evacuation Zone with ID {Dto.ZoneId} not found.");

            var ETA = Calculate.CalculateETA(EvacuationZoneData.Latitude, EvacuationZoneData.Longitude, VehiclesData.Latitude, VehiclesData.Longitude, VehiclesData.Speed);
            string stringETA = Calculate.GetRemainingTimeString(ETA);

            var dataLog = new EvacuationLog
            {
                ZoneId = Dto.ZoneId,
                VehicleId = Dto.VehicleId,
                EvacuatedCount = Dto.EvacuatedCount,
                EvacuationTime = Dto.EvacuationTime,
                Status = Dto.Status.ToString(),
                Eta = stringETA,
                CreateDate = dateTime,
            };

            var jsonData = JsonSerializer.Serialize(dataLog);

            var redisLogData = new RedisSyncData
            {
                Key = GlobalConstants.RedisEvacuationsLog + ":" + Dto.ZoneId + ":" + Dto.VehicleId + ":" + dateTime.ToString("yyyyMMddHHmmss"),
                Value = jsonData,
                UpdateTime = dateTime,
            };

            var dataPlan = new EvacuationsPlan
            {
                ZoneId = Dto.ZoneId,
                VehicleId = Dto.VehicleId,
                Eta = null,
                Status = Dto.Status.ToString(),
                UpdateDate = dateTime
            };

            var jsonPlanData = JsonSerializer.Serialize(dataPlan);

            var keyPlanRedis = GlobalConstants.RedisEvacuationsPlan + ":" + dataLog.ZoneId + ":" + dataLog.VehicleId;

            await _RedisServices.CreateAsync(redisLogData, GlobalConstants.RedisEvacuationsLog);
            await _EvacuationsServices.UpdatePlanRedisAsync(keyPlanRedis, dataPlan);

            var evacuationPlanData = await _EvacuationsServices.GetAllPlanAsyncRedis();
            var rawPlanData = evacuationPlanData.Where(x => x.ZoneId == dataLog.ZoneId && x.IsDelete != true).ToList();
            var rawPlanDataCompleted = evacuationPlanData.Where(x => x.Status == "ZONE_COMPLETED").ToList().OrderByDescending(x => x.CreateDate);
            var status = rawPlanData.Select(x => x.Status).OrderBy(s => statusPriority.IndexOf(s)).FirstOrDefault();

            int? numberPeople = 0;

            foreach (var item in rawPlanDataCompleted)
            {
                numberPeople += item.EvacuatedCount;
            }

            if(EvacuationZoneData.NumberPeople == numberPeople)
            {
                var dataZone = new EvacuationZone
                {
                    ZoneId = Dto.ZoneId,
                    Status = "ZONE_COMPLETED",
                    UpdateDate = dateTime
                };

                await _EvacuationZonesServices.UpdateRedisAsync(GlobalConstants.RedisEvacuationsZone + ":" + Dto.ZoneId, dataZone);
            }
            else
            {
                var dataZone = new EvacuationZone
                {
                    ZoneId = Dto.ZoneId,
                    Status = status,
                    UpdateDate = dateTime
                };

                if(Dto.Status == GlobalConstants.ZoneStatus.ZONE_COMPLETED)
                {
                    dataZone.Status = "ZONE_EVACUATING";
                }

                await _EvacuationZonesServices.UpdateRedisAsync(GlobalConstants.RedisEvacuationsZone + ":" + Dto.ZoneId, dataZone);
            }

            return NoContent();
        }

        [HttpGet]
        [Route("/api/evacuations/status")]
        public async Task<IActionResult> GetStatus()
        {
            var data = new List<DisplayEvacuationsSatatus>();

            var evacuationZonesData = await _EvacuationZonesServices.GetAllAsyncRedis();
            var evacuationPlanData = await _EvacuationsServices.GetAllPlanAsyncRedis();
            evacuationZonesData = evacuationZonesData.Where(x => x.IsDelete != true).ToList();
            if (evacuationZonesData.Any())
            {
                foreach (var zone in evacuationZonesData)
                {
                    var completedZone = evacuationPlanData.Where(x => x.ZoneId == zone.ZoneId && x.Status == "ZONE_COMPLETED" && x.IsDelete != true).OrderByDescending(x=>x.CreateDate);
                    var lastCar = completedZone.FirstOrDefault()?.VehicleId;
                    var totalEvacuated = completedZone.Sum(x => x.EvacuatedCount);
                    var remainingPeople = zone.NumberPeople - totalEvacuated;
                    
                    data.Add(new DisplayEvacuationsSatatus
                    {
                        ZoneId = zone.ZoneId,
                        TotalEvacuated = totalEvacuated,
                        RemainingPeople = remainingPeople,
                        LastVehicleUsed = lastCar,
                        Status = zone.Status,
                        UpdateTime = zone.UpdateDate,
                    });
                }
            }

            return Ok(data);
        }

        [HttpDelete]
        [Route("/api/evacuations/clear")]
        public async Task<IActionResult> ClearEvacuations()
        {
            await _EvacuationsServices.DeletePlanAsync();
            await _EvacuationZonesServices.DeleteZoneAsync();
            await _VehiclesServices.DeleteAllAsync();
            return NoContent();
        }

    }
}
