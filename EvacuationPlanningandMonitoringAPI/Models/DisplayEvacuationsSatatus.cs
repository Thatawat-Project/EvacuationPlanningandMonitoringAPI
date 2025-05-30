using EvacuationPlanningandMonitoringAPI.Helpers;

namespace EvacuationPlanningandMonitoringAPI.Models
{
    public class DisplayEvacuationsSatatus
    {
        public string ZoneId { get; set; }
        public int? TotalEvacuated { get; set; }
        public int? RemainingPeople { get; set; }
        public string? LastVehicleUsed { get; set; }
        public string Status { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
