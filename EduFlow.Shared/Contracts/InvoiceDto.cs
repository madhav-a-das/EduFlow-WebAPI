namespace EduFlow.Shared.Contracts;

/// <summary>Returned by FinanceService (M4).</summary>
public class InvoiceDto
{
    public int InvoiceID { get; set; }
    public int StudentID { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public DateTime IssuedAt { get; set; }
    public DateTime DueDate { get; set; }

    /// <summary>Pending | Paid | Overdue | Cancelled</summary>
    public string Status { get; set; } = string.Empty;

    public string? InvoiceURI { get; set; }
}
