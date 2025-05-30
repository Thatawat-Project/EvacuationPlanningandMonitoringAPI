namespace EvacuationPlanningandMonitoringAPI.Models.DTOs
{
    public class EvacuationsUpdateDto
    {
        public string ZoneId { get; set; }
        public string VehicleId { get; set; }
        public int EvacuatedCount { get; set; }
        public DateTime evacuationTime { get; set; }
        public string Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
