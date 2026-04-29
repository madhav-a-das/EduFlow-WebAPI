using AttendanceTracking.Models;

namespace AttendanceTracking.Repositories.Interfaces
{
    public interface IAttendanceRepository
    {
        void Add(Attendance attendance);
        Attendance GetById(int id);
        double CalculateAttendancePercentage(int studentId, int courseId);

        bool Exists(int studentId, int courseId, DateTime date);
        IEnumerable<Attendance> GetByStudentId(int studentId);
        IEnumerable<Attendance> GetByCourseId(int courseId);
        IEnumerable<Attendance> GetByDateRange(DateTime from, DateTime to);
        IEnumerable<Attendance> GetAll();
    }
}