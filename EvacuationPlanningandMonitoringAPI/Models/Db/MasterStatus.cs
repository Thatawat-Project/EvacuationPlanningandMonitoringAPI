using System;
using System.Collections.Generic;

namespace EvacuationPlanningandMonitoringAPI.Models.Db;

public partial class MasterStatus
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Category { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }
}
