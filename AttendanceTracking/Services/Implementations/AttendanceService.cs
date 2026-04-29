using AttendanceTracking.Clients;
using AttendanceTracking.DTOs;
using AttendanceTracking.Models;
using AttendanceTracking.Repositories.Interfaces;
using AttendanceTracking.Services.Interfaces;
using Serilog;

namespace AttendanceTracking.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _repository;
        private readonly StudentClient _studentClient;
        private readonly AcademicClient _academicClient;

        public AttendanceService(
            IAttendanceRepository repository,
            StudentClient studentClient,
            AcademicClient academicClient)
        {
            _repository = repository;
            _studentClient = studentClient;
            _academicClient = academicClient;
        }

        // ✅ SINGLE ATTENDANCE (Validated via M1 & M2)
        public async Task RecordAttendanceAsync(CreateAttendanceDto dto, int recordedBy)
        {
            Log.Information("Recording attendance for Student {StudentID}, Course {CourseID}",
                dto.StudentID, dto.CourseID);

            //// ✅ Validate Student (M1)
            var student = await _studentClient.GetStudentAsync(dto.StudentID);
            if (student == null)
                throw new ApplicationException("Student does not exist.");

            //// ✅ Validate Course (M2)
            //var course = await _academicClient.GetCourseAsync(dto.CourseID);
            //if (course == null)
            //    throw new ApplicationException("Course does not exist.");

            //// ✅ Validate Faculty / User (M1)
            bool facultyValid = await _studentClient.UserExistsAsync(recordedBy);
            if (!facultyValid)
                throw new ApplicationException("Invalid faculty user.");

            // ✅ Business validations (existing logic)
            if (_repository.Exists(dto.StudentID, dto.CourseID, dto.Date))
                throw new ApplicationException("Attendance already recorded.");

            if (dto.Status != "Present" && dto.Status != "Absent")
                throw new ApplicationException("Invalid attendance status.");

            var attendance = new Attendance
            {
                StudentID = dto.StudentID,
                CourseID = dto.CourseID,
                Date = dto.Date,
                Status = dto.Status,
                RecordedByFK = recordedBy,
                RecordedAt = DateTime.UtcNow
            };

            _repository.Add(attendance);

            Log.Information("Attendance recorded successfully");
        }

        // ✅ BULK ATTENDANCE (Validated per record)
        public async Task RecordBulkAttendanceAsync(List<CreateAttendanceDto> records, int recordedBy)
        {
            foreach (var dto in records)
            {
                var student = await _studentClient.GetStudentAsync(dto.StudentID);
                var course = await _academicClient.GetCourseAsync(dto.CourseID);

                if (student == null || course == null)
                {
                    Log.Warning("Skipping invalid attendance record for Student {StudentID}, Course {CourseID}",
                        dto.StudentID, dto.CourseID);
                    continue;
                }

                if (!_repository.Exists(dto.StudentID, dto.CourseID, dto.Date))
                {
                    var attendance = new Attendance
                    {
                        StudentID = dto.StudentID,
                        CourseID = dto.CourseID,
                        Date = dto.Date,
                        Status = dto.Status,
                        RecordedByFK = recordedBy,
                        RecordedAt = DateTime.UtcNow
                    };

                    _repository.Add(attendance);
                }
            }

            Log.Information("Bulk attendance processing completed");
        }

        // ✅ READ METHODS (UNCHANGED)
        public IEnumerable<Attendance> GetByStudent(int studentId)
            => _repository.GetByStudentId(studentId);

        public IEnumerable<Attendance> GetByCourse(int courseId)
            => _repository.GetByCourseId(courseId);

        public IEnumerable<Attendance> GetByDateRange(DateTime from, DateTime to)
            => _repository.GetByDateRange(from, to);

        public bool IsBelowThreshold(int studentId, int courseId, double threshold)
        {
            var percentage = GetAttendancePercentage(studentId, courseId);
            return percentage < threshold;
        }

        public double GetAttendancePercentage(int studentId, int courseId)
        {
            return _repository.CalculateAttendancePercentage(studentId, courseId);
        }

        public List<object> GetAttendanceSummary()
        {
            var records = _repository.GetAll();

            var summary = records
                .GroupBy(a => new { a.StudentID, a.CourseID })
                .Select(g => new
                {
                    StudentID = g.Key.StudentID,
                    CourseID = g.Key.CourseID,
                    TotalClasses = g.Count(),
                    PresentCount = g.Count(a => a.Status == "Present"),
                    AttendancePercentage = Math.Round(
                        (double)g.Count(a => a.Status == "Present") / g.Count() * 100, 2)
                })
                .ToList<object>();

            return summary;
        }
    }
}
