using System.Net.Http.Json;
using ReportingService.DTOs;

namespace ReportingService.Clients
{
    public class GradingClient
    {
        private readonly HttpClient _httpClient;

        public GradingClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<GradeDto>> GetAllGradesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<GradeDto>>("api/Grade");
            return result ?? new List<GradeDto>();
        }

        public async Task<List<GradeDto>> GetGradesBySemesterAsync(string semester)
        {
            var result = await _httpClient.GetFromJsonAsync<List<GradeDto>>($"api/Grade/semester/{semester}");
            return result ?? new List<GradeDto>();
        }
    }
}