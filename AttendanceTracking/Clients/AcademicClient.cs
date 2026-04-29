using System.Net.Http.Json;
using AttendanceTracking.DTOs;
using Serilog;

namespace AttendanceTracking.Clients
{
    public class AcademicClient
    {
        private readonly HttpClient _httpClient;

        public AcademicClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Used to validate course existence and fetch course details
        /// </summary>
        public async Task<CourseDto?> GetCourseAsync(int courseId)
        {
            Log.Information("Calling AcademicService to fetch CourseID {CourseID}", courseId);

            return await _httpClient.GetFromJsonAsync<CourseDto>(
                $"/api/courses/{courseId}");
        }
    }
}
