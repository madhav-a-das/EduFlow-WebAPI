namespace ReportingService.DTOs
{
    public class EnrolmentDto
    {
        public int EnrollID { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public string Semester { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}