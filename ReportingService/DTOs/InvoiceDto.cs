namespace ReportingService.DTOs
{
    public class InvoiceDto
    {
        public int InvoiceID { get; set; }
        public int StudentID { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}