namespace EduFlow.Shared.Contracts;

/// <summary>
/// Returned by AttendanceTracking (M3) GET /api/attendance/summary.
///
/// Aggregated per (StudentID, CourseID) pair. The endpoint computes this
/// from the raw Attendance rows so M8 can power its academic dashboard
/// without hitting M3 once per student.
/// </summary>
public class AttendanceSummaryDto
{
    public int StudentID { get; set; }
    public int CourseID { get; set; }
    public int TotalClasses { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public double AttendancePercentage { get; set; }
}
