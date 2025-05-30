namespace EvacuationPlanningandMonitoringAPI.Helpers
{
    public class Calculate
    {
        private static double ToRadians(double deg) => deg * (Math.PI / 180);
        public static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371;
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        public static DateTime CalculateETA(decimal? lat1, decimal? lon1, decimal? lat2, decimal? lon2, decimal? speedKmPerHour)
        {
            if (!lat1.HasValue || !lon1.HasValue || !lat2.HasValue || !lon2.HasValue || !speedKmPerHour.HasValue || speedKmPerHour <= 0)
            {
                return DateTime.MinValue;
            }

            double dLat1 = (double)lat1.Value;
            double dLon1 = (double)lon1.Value;
            double dLat2 = (double)lat2.Value;
            double dLon2 = (double)lon2.Value;
            double dSpeed = (double)speedKmPerHour.Value;

            var distanceKm = CalculateDistanceKm(dLat1, dLon1, dLat2, dLon2);
            var travelTimeHours = distanceKm / dSpeed;
            return DateTime.Now.AddHours(travelTimeHours);
        }

        public static string GetRemainingTimeString(DateTime targetTime)
        {
            var remaining = targetTime - DateTime.Now;

            if (remaining.TotalSeconds <= 0)
            {
                return "00:00:00";
            }

            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                remaining.Hours + remaining.Days * 24,
                remaining.Minutes,
                remaining.Seconds);
        }

        public static string ConvertTimeStringToReadable(string timeString)
        {
            if (TimeSpan.TryParse(timeString, out TimeSpan timeSpan))
            {
                int hours = timeSpan.Hours + timeSpan.Days * 24;
                int minutes = timeSpan.Minutes;
                int seconds = timeSpan.Seconds;

                return $"{hours} hours {minutes} minutes {seconds} seconds";
            }
            else
            {
                return "Invalid time format";
            }
        }

    }
}
