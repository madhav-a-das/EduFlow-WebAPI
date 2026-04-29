using System.Net.Http.Json;
using ReportingService.DTOs;

namespace ReportingService.Clients
{
    public class AcademicClient
    {
        private readonly HttpClient _httpClient;

        public AcademicClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get all courses from M2
        public async Task<List<CourseDto>> GetAllCoursesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<CourseDto>>("api/Course");
            return result ?? new List<CourseDto>();
        }

        // Get a single course by ID from M2
        public async Task<CourseDto?> GetCourseByIdAsync(int courseId)
        {
            return await _httpClient.GetFromJsonAsync<CourseDto>($"api/Course/{courseId}");
        }
    }
}