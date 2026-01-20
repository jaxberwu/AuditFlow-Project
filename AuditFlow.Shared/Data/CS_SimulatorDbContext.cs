using Microsoft.EntityFrameworkCore;
using AuditFlow.Shared.Entities;

namespace AuditFlow.Shared.Data;

public class CS_SimulatorDbContext : DbContext
{
    public CS_SimulatorDbContext(DbContextOptions<CS_SimulatorDbContext> options) : base(options)
    {
    }

    public DbSet<CVEThreat> CVEThreats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CVEThreat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Hostname).HasMaxLength(200).IsRequired();
            entity.Property(e => e.CVE_ID).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Severity).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Remediation).HasMaxLength(500);
            entity.HasIndex(e => e.Hostname);
        });
    }
}
