namespace ReportingService.DTOs
{
    public class GradeDto
    {
        public int GradeID { get; set; }
        public int EnrollmentID { get; set; }
        public decimal MarksObtained { get; set; }
        public string GradeLetter { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
    }
}