using System.Net.Http.Json;
using ReportingService.DTOs;

namespace ReportingService.Clients
{
    public class FinanceClient
    {
        private readonly HttpClient _httpClient;

        public FinanceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get all invoices from M4
        public async Task<List<InvoiceDto>> GetAllInvoicesAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<InvoiceDto>>("api/Invoice");
            return result ?? new List<InvoiceDto>();
        }

        // Get all payments from M4
        public async Task<List<PaymentDto>> GetAllPaymentsAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<PaymentDto>>("api/Invoice/payments");
            return result ?? new List<PaymentDto>();
        }

        // Get invoices for a specific student from M4
        public async Task<List<InvoiceDto>> GetInvoicesByStudentAsync(int studentId)
        {
            var result = await _httpClient.GetFromJsonAsync<List<InvoiceDto>>($"api/Invoice/student/{studentId}");
            return result ?? new List<InvoiceDto>();
        }
    }
}