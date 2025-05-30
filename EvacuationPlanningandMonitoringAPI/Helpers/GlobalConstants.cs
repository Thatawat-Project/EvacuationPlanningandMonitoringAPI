namespace EvacuationPlanningandMonitoringAPI.Helpers
{
    public static class GlobalConstants
    {
        public const string RedisEvacuationsZone = "EvacuationsZone";
        public const string RedisVehicleKey = "Vehicle";
        public const string RedisEvacuationsLog = "EvacuationsLog";
        public const string RedisEvacuationsPlan = "EvacuationsPlan";
        public enum ZoneStatus
        {
            ZONE_PENDING,
            ZONE_EVACUATING,
            ZONE_COMPLETED,
            ZONE_FAILED,
            ZONE_CANCELED
        }

        public enum VehiclesStatus
        {
            AVAILABLE,
            UNAVAILABLE
        }
        public class Coordinates
        {
            public decimal? Latitude { get; set; }
            public decimal? Longitude { get; set; }
        }
    }
}
