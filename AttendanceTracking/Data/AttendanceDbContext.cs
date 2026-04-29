using Microsoft.EntityFrameworkCore;
using AttendanceTracking.Models;

namespace AttendanceTracking.Data
{
    public class AttendanceDbContext : DbContext
    {
        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new { a.StudentID, a.CourseID, a.Date })
                .IsUnique();
        }

        public DbSet<Attendance> Attendances { get; set; }
    }
}
