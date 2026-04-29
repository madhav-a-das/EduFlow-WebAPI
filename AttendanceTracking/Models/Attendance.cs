
namespace AttendanceTracking.Models
{
    public class Attendance
    {
        public int AttendanceID { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public int RecordedByFK { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
