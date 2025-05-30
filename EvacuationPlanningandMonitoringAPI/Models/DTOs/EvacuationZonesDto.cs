using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EvacuationPlanningandMonitoringAPI.Helpers;

namespace EvacuationPlanningandMonitoringAPI.Models.DTOs
{
    public class EvacuationZonesDto
    {
        [Required]
        public string ZoneId { get; set; } = null!;
        [Required]
        public string? NameZone { get; set; }
        [Required]
        public int? NumberPeople { get; set; }
        [Range(1, 5)]
        public int? UrgencyLevel { get; set; }
        [Required]
        public decimal? Latitude { get; set; }
        [Required]
        public decimal? Longitude { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GlobalConstants.ZoneStatus Status { get; set; }
    }
}
