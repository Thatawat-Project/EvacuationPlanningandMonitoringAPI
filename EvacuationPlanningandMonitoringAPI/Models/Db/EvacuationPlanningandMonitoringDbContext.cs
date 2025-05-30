using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EvacuationPlanningandMonitoringAPI.Models.Db;

public partial class EvacuationPlanningandMonitoringDbContext : DbContext
{
    public EvacuationPlanningandMonitoringDbContext()
    {
    }

    public EvacuationPlanningandMonitoringDbContext(DbContextOptions<EvacuationPlanningandMonitoringDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EvacuationLog> EvacuationLogs { get; set; }

    public virtual DbSet<EvacuationZone> EvacuationZones { get; set; }

    public virtual DbSet<EvacuationsPlan> EvacuationsPlans { get; set; }

    public virtual DbSet<MasterStatus> MasterStatuses { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=ILJIBEARz;Database=EvacuationPlanningandMonitoringDB;User ID=sa;Password=1234;Trusted_Connection=TRUE;encrypt=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EvacuationLog>(entity =>
        {
            entity.ToTable("EvacuationLog");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Eta)
                .HasMaxLength(50)
                .HasColumnName("ETA");
            entity.Property(e => e.EvacuationTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.VehicleId).HasMaxLength(10);
            entity.Property(e => e.ZoneId).HasMaxLength(10);
        });

        modelBuilder.Entity<EvacuationZone>(entity =>
        {
            entity.HasKey(e => e.ZoneId).HasName("PK_MasterEvacuationZones");

            entity.Property(e => e.ZoneId).HasMaxLength(10);
            entity.Property(e => e.CreateBy).HasMaxLength(200);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(false);
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.Latitude).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.NameZone).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdateBy).HasMaxLength(200);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EvacuationsPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_EvacuationPlan");

            entity.ToTable("EvacuationsPlan");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Eta)
                .HasMaxLength(50)
                .HasColumnName("ETA");
            entity.Property(e => e.EvacuationTarget).HasMaxLength(50);
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
            entity.Property(e => e.VehicleId).HasMaxLength(10);
            entity.Property(e => e.ZoneId).HasMaxLength(10);
        });

        modelBuilder.Entity<MasterStatus>(entity =>
        {
            entity.ToTable("MasterStatus");

            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(false)
                .HasColumnName("isActive");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK_MasterVehicles");

            entity.Property(e => e.VehicleId)
                .HasMaxLength(10)
                .HasColumnName("VehicleID");
            entity.Property(e => e.CreateBy).HasMaxLength(200);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(false);
            entity.Property(e => e.IsDelete).HasDefaultValue(false);
            entity.Property(e => e.Latitude).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Speed).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.UpdateBy).HasMaxLength(200);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
