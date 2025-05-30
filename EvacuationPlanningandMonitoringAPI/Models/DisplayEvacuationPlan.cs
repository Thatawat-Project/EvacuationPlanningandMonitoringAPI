namespace EvacuationPlanningandMonitoringAPI.Models
{
    public class DisplayEvacuationPlan
    {
        public string? ZoneId { get; set; }

        public string? VehicleId { get; set; }

        public string ETA { get; set; }

        public string? EvacuationTarget { get; set; }

        public int? NumberOfPeople { get; set; }

        public int? TripNumber { get; set; }

        public string? Status { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
