using System.Text.Json;
using EvacuationPlanningandMonitoringAPI.Helpers;
using EvacuationPlanningandMonitoringAPI.Models;
using EvacuationPlanningandMonitoringAPI.Models.Db;
using EvacuationPlanningandMonitoringAPI.Models.DTOs;
using EvacuationPlanningandMonitoringAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EvacuationPlanningandMonitoringAPI.Controllers
{
    [ApiController]
    [Route("api/evacuation-zones")]
    public class EvacuationZonesController : ControllerBase
    {

        private readonly IEvacuationZonesServices _evacuationzonesservice;
        private readonly IEvacuationsServices _evacuationsservice;
        private readonly IRedisServices _redisservices;

        public EvacuationZonesController(IEvacuationZonesServices evacuationzonesservice, IEvacuationsServices evacuationsservice, IRedisServices redisservices)
        {
            _evacuationzonesservice = evacuationzonesservice;
            _evacuationsservice = evacuationsservice;
            _redisservices = redisservices;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EvacuationZonesDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var DupData = _evacuationzonesservice.GetAllAsync().Result.Where(x=>x.ZoneId.Contains(dto.ZoneId));

            if(DupData.Any())
            {
                return BadRequest("Duplicate ZoneId detected.");
            }

            var dateTime = DateTime.Now;

            var zoneData = new EvacuationZone
            {
                ZoneId = dto.ZoneId,
                NameZone = dto.NameZone,
                UrgencyLevel = dto.UrgencyLevel,
                NumberPeople = dto.NumberPeople,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Status = dto.Status.ToString(),
                CreateDate = dateTime,
                UpdateDate = dateTime
            };

            var statusData = new EvacuationsStatusDto
            {
                ZoneId = dto.ZoneId,
                TotalEvacuated = 0,
                RemainingPeople = dto.NumberPeople,
                UpdateTime = dateTime
            };

            var jsonData = JsonSerializer.Serialize(zoneData);

            var redisData = new RedisSyncData
            {
                Key = GlobalConstants.RedisEvacuationsZone + ":" + zoneData.ZoneId,
                Value = jsonData,
                UpdateTime = dateTime
            };

            await _evacuationzonesservice.CreateAsync(zoneData);
            await _redisservices.CreateAsync(redisData, GlobalConstants.RedisEvacuationsZone);

            return Ok(zoneData);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var rawData = await _evacuationzonesservice.GetAllAsyncRedis();
            rawData = rawData.Where(x => x.IsDelete != true).OrderBy(x => x.CreateDate).ToList();

            var data = new List<DisplayEvacuationsZone>();

            foreach (var item in rawData)
            {
                var coordinates = new GlobalConstants.Coordinates
                {
                    Latitude = item.Latitude,
                    Longitude = item.Longitude
                };
                var zoneData = new DisplayEvacuationsZone
                {
                    ZoneId = item.ZoneId,
                    NameZone = item.NameZone,
                    NumberPeople = item.NumberPeople,
                    UrgencyLevel = item.UrgencyLevel,
                    LocationCoordinates = coordinates,
                    Status = item.Status,
                    CreateDate = item.CreateDate,
                    UpdateDate = item.UpdateDate
                };

                data.Add(zoneData);
            }

            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var rawData = await _evacuationzonesservice.GetByIdAsyncRedis(id);
            if (rawData == null) return NotFound();

            var coordinates = new GlobalConstants.Coordinates
            {
                Latitude = rawData.Latitude,
                Longitude = rawData.Longitude
            };

            var data = new DisplayEvacuationsZone
            {
                ZoneId = rawData.ZoneId,
                NameZone = rawData.NameZone,
                NumberPeople = rawData.NumberPeople,
                UrgencyLevel = rawData.UrgencyLevel,
                LocationCoordinates = coordinates,
                Status = rawData.Status,
                CreateDate = rawData.CreateDate,
                UpdateDate = rawData.UpdateDate
            };

            return Ok(data);
        }
    }
}
