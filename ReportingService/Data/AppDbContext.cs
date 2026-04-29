
using Microsoft.EntityFrameworkCore;
using ReportingService.Models;

namespace ReportingService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Each DbSet = one table in the database
        public DbSet<Report> Reports { get; set; }
        public DbSet<KPI> KPIs { get; set; }
        public DbSet<AuditPackage> AuditPackages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Report table rules ──
            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.ReportID);
                entity.Property(e => e.Scope).IsRequired().HasMaxLength(20);
                entity.Property(e => e.GeneratedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // ── KPI table rules ──
            modelBuilder.Entity<KPI>(entity =>
            {
                entity.HasKey(e => e.KPIID);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // ── AuditPackage table rules ──
            modelBuilder.Entity<AuditPackage>(entity =>
            {
                entity.HasKey(e => e.PackageID);
                entity.Property(e => e.GeneratedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}