using System.Net.Http.Json;
using ReportingService.DTOs;

namespace ReportingService.Clients
{
    public class AttendanceClient
    {
        private readonly HttpClient _httpClient;

        public AttendanceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AttendanceSummaryDto>> GetAttendanceSummaryAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<AttendanceSummaryDto>>(
                "api/attendance/summary");
            return result ?? new List<AttendanceSummaryDto>();
        }
    }
}