using System.Net.Http.Json;
using ReportingService.DTOs;

namespace ReportingService.Clients
{
    public class StudentClient
    {
        private readonly HttpClient _httpClient;

        public StudentClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/user/internal/all");
            request.Headers.Add("X-API-Key", "eduflow-internal-key-2025");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<UserDto>>();
            return result ?? new List<UserDto>();
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            return await _httpClient.GetFromJsonAsync<UserDto>($"api/user/{userId}");
        }

        public async Task<List<UserDto>> GetUsersByRoleAsync(string role)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/user/internal/role/{role}");
            request.Headers.Add("X-API-Key", "eduflow-internal-key-2025");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<UserDto>>();
            return result ?? new List<UserDto>();
        }
    }
}