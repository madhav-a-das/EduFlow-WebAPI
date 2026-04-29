using AttendanceTracking.DTOs;
using AttendanceTracking.Models;

namespace AttendanceTracking.Services.Interfaces
{
    public interface IAttendanceService
    {
        
        Task RecordAttendanceAsync(CreateAttendanceDto dto, int recordedBy);
        Task RecordBulkAttendanceAsync(List<CreateAttendanceDto> records, int recordedBy);

        IEnumerable<Attendance> GetByStudent(int studentId);
        IEnumerable<Attendance> GetByCourse(int courseId);
        IEnumerable<Attendance> GetByDateRange(DateTime from, DateTime to);

        double GetAttendancePercentage(int studentId, int courseId);
        bool IsBelowThreshold(int studentId, int courseId, double threshold);
        List<object> GetAttendanceSummary();
    }
}
