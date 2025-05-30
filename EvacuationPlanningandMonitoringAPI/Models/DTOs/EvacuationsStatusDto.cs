namespace EvacuationPlanningandMonitoringAPI.Models.DTOs
{
    public class EvacuationsStatusDto
    {
        public string? ZoneId { get; set; }

        public int? TotalEvacuated { get; set; }

        public int? RemainingPeople { get; set; }

        public string? LastVehicleUsed { get; set; }

        public DateTime? UpdateTime { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
