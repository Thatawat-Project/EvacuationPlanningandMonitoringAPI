using System;
using System.Collections.Generic;

namespace EvacuationPlanningandMonitoringAPI.Models.Db;

public partial class EvacuationLog
{
    public long Id { get; set; }

    public string? ZoneId { get; set; }

    public string? VehicleId { get; set; }

    public int? EvacuatedCount { get; set; }

    public DateTime? EvacuationTime { get; set; }

    public string? Status { get; set; }

    public string? Eta { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public DateTime? CreateDate { get; set; }
}
