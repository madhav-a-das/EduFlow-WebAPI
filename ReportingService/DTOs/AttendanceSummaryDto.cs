namespace ReportingService.DTOs
{
    public class AttendanceSummaryDto
    {
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public int TotalClasses { get; set; }
        public int PresentCount { get; set; }
        public double AttendancePercentage { get; set; }
    }
}