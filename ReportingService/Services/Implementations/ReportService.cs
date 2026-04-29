using ReportingService.Models;
using ReportingService.Clients;
using ReportingService.DTOs;
using ReportingService.Repositories.Interfaces;
using ReportingService.Services.Interfaces;

namespace ReportingService.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepo;
        private readonly StudentClient _studentClient;
        private readonly AcademicClient _academicClient;
        private readonly AttendanceClient _attendanceClient;
        private readonly GradingClient _gradingClient;
        private readonly FinanceClient _financeClient;

        public ReportService(
            IReportRepository reportRepo,
            StudentClient studentClient,
            AcademicClient academicClient,
            AttendanceClient attendanceClient,
            GradingClient gradingClient,
            FinanceClient financeClient)
        {
            _reportRepo = reportRepo;
            _studentClient = studentClient;
            _academicClient = academicClient;
            _attendanceClient = attendanceClient;
            _gradingClient = gradingClient;
            _financeClient = financeClient;
        }

        // =============================================
        //  CRUD — delegates to repository
        // =============================================

        public async Task<IEnumerable<Report>> GetAllAsync()
        {
            return await _reportRepo.GetAllAsync();
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _reportRepo.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Report>> GetByScopeAsync(string scope)
        {
            return await _reportRepo.GetByScopeAsync(scope);
        }

        public async Task<Report> CreateAsync(Report report)
        {
            return await _reportRepo.CreateAsync(report);
        }

        public async Task UpdateAsync(Report report)
        {
            await _reportRepo.UpdateAsync(report);
        }

        public async Task DeleteAsync(int id)
        {
            await _reportRepo.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _reportRepo.ExistsAsync(id);
        }

        // =============================================
        //  DASHBOARDS — each service call wrapped separately
        // =============================================

        public async Task<object> GetEnrollmentDashboardAsync()
        {
            var allUsers = new List<UserDto>();
            var students = new List<UserDto>();
            var courses = new List<CourseDto>();

            // ── Call M1: IdentityService ──
            try
            {
                allUsers = await _studentClient.GetAllUsersAsync();
                students = await _studentClient.GetUsersByRoleAsync("Student");
            }
            catch (HttpRequestException)
            {
                // IdentityService is not running — continue with empty data
            }

            // ── Call M2: AcademicService ──
            try
            {
                courses = await _academicClient.GetAllCoursesAsync();
            }
            catch (HttpRequestException)
            {
                // AcademicService is not running — continue with empty data
            }

            return new
            {
                TotalUsers = allUsers.Count,
                TotalStudents = students.Count,
                ActiveStudents = students.Count(s => s.Status == "Active"),
                StudentsByStatus = students
                    .GroupBy(s => s.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() }),
                TotalCourses = courses.Count,
                ActiveCourses = courses.Count(c => c.Status == "Active"),
                IdentityServiceAvailable = allUsers.Any(),
                AcademicServiceAvailable = courses.Any()
            };
        }

        public async Task<object> GetAcademicDashboardAsync()
        {
            var attendance = new List<AttendanceSummaryDto>();
            var grades = new List<GradeDto>();

            // ── Call M3: AttendanceService ──
            try
            {
                attendance = await _attendanceClient.GetAttendanceSummaryAsync();
            }
            catch (HttpRequestException)
            {
                // AttendanceService is not running — continue with empty data
            }

            // ── Call M6: GradingService ──
            try
            {
                grades = await _gradingClient.GetAllGradesAsync();
            }
            catch (HttpRequestException)
            {
                // GradingService is not running — continue with empty data
            }

            return new
            {
                TotalGradesRecorded = grades.Count,
                AverageMarks = grades.Any() ? grades.Average(g => g.MarksObtained) : 0,
                GradeDistribution = grades
                    .GroupBy(g => g.GradeLetter)
                    .Select(g => new { Grade = g.Key, Count = g.Count() }),
                AverageAttendance = attendance.Any() ? attendance.Average(a => a.AttendancePercentage) : 0,
                LowAttendanceCount = attendance.Count(a => a.AttendancePercentage < 75),
                AttendanceServiceAvailable = attendance.Any(),
                GradingServiceAvailable = grades.Any()
            };
        }

        public async Task<object> GetFinanceDashboardAsync()
        {
            var invoices = new List<InvoiceDto>();
            var payments = new List<PaymentDto>();

            // ── Call M4: FinanceService ──
            try
            {
                invoices = await _financeClient.GetAllInvoicesAsync();
                payments = await _financeClient.GetAllPaymentsAsync();
            }
            catch (HttpRequestException)
            {
                // FinanceService is not running — continue with empty data
            }

            return new
            {
                TotalInvoices = invoices.Count,
                TotalInvoicedAmount = invoices.Sum(i => i.Amount),
                PaidInvoices = invoices.Count(i => i.Status == "Paid"),
                PendingInvoices = invoices.Count(i => i.Status == "Pending"),
                TotalPaymentsReceived = payments.Sum(p => p.Amount),
                OutstandingAmount = invoices.Where(i => i.Status != "Paid").Sum(i => i.Amount),
                FinanceServiceAvailable = invoices.Any()
            };
        }
    }
}