using System.Text.Json;
using System.Xml.Linq;
using EvacuationPlanningandMonitoringAPI.Helpers;
using EvacuationPlanningandMonitoringAPI.Models;
using EvacuationPlanningandMonitoringAPI.Models.Db;
using EvacuationPlanningandMonitoringAPI.Models.DTOs;
using EvacuationPlanningandMonitoringAPI.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using static EvacuationPlanningandMonitoringAPI.Models.DisplayVehicles;

namespace EvacuationPlanningandMonitoringAPI.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehiclesServices _vehiclesservice;
        private readonly IRedisServices _redisServices;
        public VehiclesController(IVehiclesServices vehiclesservice, IRedisServices redisServices)
        {
            _vehiclesservice = vehiclesservice;
            _redisServices = redisServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rawData = await _vehiclesservice.GetAllAsyncRedis();
            rawData = rawData.Where(x => x.IsDelete != true).OrderBy(x => x.CreateDate).ToList();
            var data = new List<DisplayVehicles>();
            foreach (var item in rawData)
            {
                var coordinates = new GlobalConstants.Coordinates
                {
                    Latitude = item.Latitude,
                    Longitude = item.Longitude
                };

                var vehiclesData = new DisplayVehicles
                {
                    VehicleId = item.VehicleId,
                    Capacity = item.Capacity,
                    Type = item.Type,
                    Speed = item.Speed,
                    LocationCoordinates = coordinates,
                    Status = item.Status,
                    CreateDate = item.CreateDate,
                    UpdateDate = item.UpdateDate
                };

                data.Add(vehiclesData);
            }

            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            //var data = await _vehiclesservice.GetByIdAsync(id);
            var rawData = await _vehiclesservice.GetByIdAsyncRedis(id);
            if (rawData == null) return NotFound();

            var coordinates = new GlobalConstants.Coordinates
            {
                Latitude = rawData.Latitude,
                Longitude = rawData.Longitude
            };

            var data = new DisplayVehicles
            {
                VehicleId = rawData.VehicleId,
                Capacity = rawData.Capacity,
                Type = rawData.Type,
                Speed = rawData.Speed,
                LocationCoordinates = coordinates,
                Status = rawData.Status,
                CreateDate = rawData.CreateDate,
                UpdateDate = rawData.UpdateDate
            };

            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VehiclesDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var DupData = _vehiclesservice.GetAllAsync().Result.Where(x => x.VehicleId.Contains(dto.VehicleId));

            if (DupData.Any()) return BadRequest("Duplicate VehicleId detected.");

            var dateTime = DateTime.Now;

            var data = new Vehicle
            {
                VehicleId = dto.VehicleId,
                Capacity = dto.Capacity,
                Type = dto.Type,
                Speed = dto.Speed,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Status = dto.Status.ToString(),
                CreateDate = dateTime,
                UpdateDate = dateTime
            };

            var jsonData = JsonSerializer.Serialize(data);

            var redisData = new RedisSyncData
            {
                Key = GlobalConstants.RedisVehicleKey+":"+data.VehicleId,
                Value = jsonData,
                UpdateTime = dateTime,
            };

            await _vehiclesservice.CreateAsync(data);
            await _redisServices.CreateAsync(redisData,GlobalConstants.RedisVehicleKey);
            return Ok(data);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] VehiclesRedisUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var data = new Vehicle
            {
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                UpdateDate = DateTime.Now
            };

            await _vehiclesservice.UpdateRedisAsync(id, data);

            return NoContent();
        }
    }
}
