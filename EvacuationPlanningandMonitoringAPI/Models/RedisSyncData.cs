namespace EvacuationPlanningandMonitoringAPI.Models
{
    public class RedisSyncData
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
