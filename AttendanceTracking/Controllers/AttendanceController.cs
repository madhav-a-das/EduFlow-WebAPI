using Microsoft.AspNetCore.Mvc;
using AttendanceTracking.DTOs;
using AttendanceTracking.Services.Interfaces;

namespace AttendanceTracking.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _service;

        public AttendanceController(IAttendanceService service)
        {
            _service = service;
        }

        //  RECORD SINGLE ATTENDANCE (ASYNC)
        [HttpPost]
        public async Task<IActionResult> Record(CreateAttendanceDto dto)
        {
            await _service.RecordAttendanceAsync(dto, recordedBy: 1);
            return Ok("Attendance recorded successfully");
        }

        //  RECORD BULK ATTENDANCE (ASYNC)
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkUpload(BulkAttendanceDto dto)
        {
            await _service.RecordBulkAttendanceAsync(dto.Records, recordedBy: 1);
            return Ok("Bulk attendance uploaded successfully.");
        }

        //  READ ENDPOINTS (UNCHANGED)
        [HttpGet("student/{studentId}")]
        public IActionResult GetByStudent(int studentId)
            => Ok(_service.GetByStudent(studentId));

        [HttpGet("course/{courseId}")]
        public IActionResult GetByCourse(int courseId)
            => Ok(_service.GetByCourse(courseId));

        [HttpGet("daterange")]
        public IActionResult GetByDateRange(DateTime from, DateTime to)
            => Ok(_service.GetByDateRange(from, to));

        //  ATTENDANCE PERCENTAGE
        [HttpGet("percentage")]
        public IActionResult GetPercentage(int studentId, int courseId)
        {
            var percentage = _service.GetAttendancePercentage(studentId, courseId);
            return Ok(new { Percentage = percentage });
        }

        //  LOW ATTENDANCE ALERT
        [HttpGet("alert")]
        public IActionResult LowAttendanceAlert(
            int studentId,
            int courseId,
            double threshold = 75)
        {
            bool isLow = _service.IsBelowThreshold(studentId, courseId, threshold);

            return Ok(new
            {
                StudentId = studentId,
                CourseId = courseId,
                IsBelowThreshold = isLow
            });
        }

        //  ATTENDANCE SUMMARY FOR REPORTING SERVICE
        [HttpGet("summary")]
        public IActionResult GetAttendanceSummary()
        {
            var summary = _service.GetAttendanceSummary();
            return Ok(summary);
        }
    }
}
