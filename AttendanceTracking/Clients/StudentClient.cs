using System.Net.Http.Json;
using AttendanceTracking.DTOs;
using Serilog;

namespace AttendanceTracking.Clients
{
    public class StudentClient
    {
        private readonly HttpClient _httpClient;
        private const string API_KEY = "eduflow-internal-key-2025";

        public StudentClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<StudentDto?> GetStudentAsync(int studentId)
        {
            Log.Information("Calling IdentityService to fetch StudentID {StudentID}", studentId);

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/user/internal/{studentId}");
            request.Headers.Add("X-API-Key", API_KEY);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<StudentDto>();
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            Log.Information("Validating user {UserID} via IdentityService", userId);

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/user/internal/{userId}");
            request.Headers.Add("X-API-Key", API_KEY);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
    }
}