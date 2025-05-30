using System;
using System.Collections.Generic;

namespace EvacuationPlanningandMonitoringAPI.Models.Db;

public partial class EvacuationsPlan
{
    public int Id { get; set; }

    public string? ZoneId { get; set; }

    public string? VehicleId { get; set; }

    public string? Eta { get; set; }

    public string? EvacuationTarget { get; set; }

    public int? EvacuatedCount { get; set; }

    public int? TripNumber { get; set; }

    public string? Status { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }
}
