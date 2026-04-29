using Microsoft.EntityFrameworkCore;
using IdentityService.Models;

namespace IdentityService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Each DbSet maps to one table in SQL Server
        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── User table ──
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);

                // Email must be unique — no two users share an email
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.Status)
                      .HasDefaultValue("Active");

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedAt)
                      .HasDefaultValueSql("GETUTCDATE()");

                // PasswordHash column exists in DB but is excluded from SELECT
                // by never mapping it to a DTO — it stays server-side only
                entity.Property(e => e.PasswordHash)
                      .IsRequired();
            });

            // ── AuditLog table ──
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.AuditID);

                entity.Property(e => e.Timestamp)
                      .HasDefaultValueSql("GETUTCDATE()");

                // Many AuditLogs → one User
                // Restrict delete: if a user is deleted we keep their audit trail
                entity.HasOne(e => e.User)
                      .WithMany(u => u.AuditLogs)
                      .HasForeignKey(e => e.UserID)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}