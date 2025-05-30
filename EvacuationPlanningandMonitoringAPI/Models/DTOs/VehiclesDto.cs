using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using EvacuationPlanningandMonitoringAPI.Helpers;

namespace EvacuationPlanningandMonitoringAPI.Models.DTOs
{
    public class VehiclesDto
    {
        [Required]
        public string VehicleId { get; set; } = null!;

        [Required]
        public int? Capacity { get; set; }

        [Required]
        public string? Type { get; set; }

        [Required]
        public decimal? Speed { get; set; }

        [Required]
        public decimal? Latitude { get; set; }

        [Required]
        public decimal? Longitude { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GlobalConstants.VehiclesStatus Status { get; set; }
    }
}
