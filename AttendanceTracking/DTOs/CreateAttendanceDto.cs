namespace AttendanceTracking.DTOs
{
    public class CreateAttendanceDto
    {
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}