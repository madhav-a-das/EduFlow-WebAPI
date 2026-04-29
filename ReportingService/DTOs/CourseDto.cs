namespace ReportingService.DTOs
{
    public class CourseDto
    {
        public int CourseID { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}