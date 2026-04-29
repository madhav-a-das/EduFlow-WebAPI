namespace ReportingService.DTOs
{
    public class PaymentDto
    {
        public int PaymentID { get; set; }
        public int InvoiceID { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}