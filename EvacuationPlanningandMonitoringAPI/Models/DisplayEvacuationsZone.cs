using EvacuationPlanningandMonitoringAPI.Helpers;

namespace EvacuationPlanningandMonitoringAPI.Models
{
    public class DisplayEvacuationsZone
    {
        public string ZoneId { get; set; } = null!;

        public string? NameZone { get; set; }

        public int? NumberPeople { get; set; }

        public int? UrgencyLevel { get; set; }

        public GlobalConstants.Coordinates LocationCoordinates { get; set; }

        public string? Status { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
