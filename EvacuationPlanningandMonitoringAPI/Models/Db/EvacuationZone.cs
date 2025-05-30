using System;
using System.Collections.Generic;

namespace EvacuationPlanningandMonitoringAPI.Models.Db;

public partial class EvacuationZone
{
    public string ZoneId { get; set; } = null!;

    public string? NameZone { get; set; }

    public int? NumberPeople { get; set; }

    public int? UrgencyLevel { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public string? CreateBy { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? UpdateBy { get; set; }

    public DateTime? UpdateDate { get; set; }
}
