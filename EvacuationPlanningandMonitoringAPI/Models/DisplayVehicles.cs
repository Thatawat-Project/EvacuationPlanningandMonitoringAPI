using EvacuationPlanningandMonitoringAPI.Helpers;

namespace EvacuationPlanningandMonitoringAPI.Models
{
    public class DisplayVehicles
    {
        public string VehicleId { get; set; } = null!;

        public int? Capacity { get; set; }

        public string? Type { get; set; }

        public decimal? Speed { get; set; }
        public GlobalConstants.Coordinates LocationCoordinates {  get; set; }

        public string? Status { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
