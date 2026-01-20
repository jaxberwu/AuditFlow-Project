using Microsoft.EntityFrameworkCore;
using AuditFlow.Shared.Entities;

namespace AuditFlow.Shared.Data;

public class AuditFlowDbContext : DbContext
{
    public AuditFlowDbContext(DbContextOptions<AuditFlowDbContext> options) : base(options)
    {
    }

    public DbSet<HardwareAsset> HardwareAssets { get; set; }
    public DbSet<ThreatAlert> ThreatAlerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<HardwareAsset>(entity =>
        {
            entity.HasKey(e => e.SN);
            entity.Property(e => e.SN).HasMaxLength(100);
            entity.Property(e => e.Hostname).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<ThreatAlert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SN).HasMaxLength(100);
            entity.Property(e => e.Severity).HasMaxLength(50);
        });
    }
}
