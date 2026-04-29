using AttendanceTracking.Data;
using AttendanceTracking.Models;
using AttendanceTracking.Repositories.Interfaces;

namespace AttendanceTracking.Repositories.Implementations
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AttendanceDbContext _context;

        public AttendanceRepository(AttendanceDbContext context)
        {
            _context = context;
        }

        public void Add(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
            _context.SaveChanges();
        }

        public Attendance GetById(int id)
            => _context.Attendances.Find(id);

        public bool Exists(int studentId, int courseId, DateTime date)
            => _context.Attendances.Any(a =>
                a.StudentID == studentId &&
                a.CourseID == courseId &&
                a.Date.Date == date.Date);

        public IEnumerable<Attendance> GetByStudentId(int studentId)
            => _context.Attendances.Where(a => a.StudentID == studentId).ToList();

        public IEnumerable<Attendance> GetByCourseId(int courseId)
            => _context.Attendances.Where(a => a.CourseID == courseId).ToList();

        public IEnumerable<Attendance> GetByDateRange(DateTime from, DateTime to)
            => _context.Attendances.Where(a =>
                a.Date.Date >= from.Date &&
                a.Date.Date <= to.Date).ToList();

        // Calculating the attendance percentage for a specific course of a student
        public double CalculateAttendancePercentage(int studentId, int courseId)
        {
            var total = _context.Attendances
                .Count(a => a.StudentID == studentId && a.CourseID == courseId);

            if (total == 0) return 0;

            var present = _context.Attendances
                .Count(a => a.StudentID == studentId &&
                            a.CourseID == courseId &&
                            a.Status == "Present");

            return (double)present / total * 100;
        }
        public IEnumerable<Attendance> GetAll()
    => _context.Attendances.ToList();
    }
}