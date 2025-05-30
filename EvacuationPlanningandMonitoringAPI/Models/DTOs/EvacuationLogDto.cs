using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EvacuationPlanningandMonitoringAPI.Helpers;

namespace EvacuationPlanningandMonitoringAPI.Models.DTOs
{
    public class EvacuationLogDto
    {
        [Required]
        public string? ZoneId { get; set; }
        [Required]
        public string? VehicleId { get; set; }
        [Required]
        public int? EvacuatedCount { get; set; }
        [Required]
        public DateTime? EvacuationTime { get; set; }
        [Required]
        public int? TripNumber { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GlobalConstants.ZoneStatus Status { get; set; }

    }
}
