using System.ComponentModel.DataAnnotations;

namespace EvacuationPlanningandMonitoringAPI.Models.DTOs
{
    public class VehiclesRedisUpdateDto
    {

        [Required]
        public decimal? Latitude { get; set; }

        [Required]
        public decimal? Longitude { get; set; }
    }
}
