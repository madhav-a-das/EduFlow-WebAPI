namespace EduFlow.Shared.Contracts;

/// <summary>Returned by FinanceService (M4).</summary>
public class PaymentDto
{
    public int PaymentID { get; set; }
    public int InvoiceID { get; set; }
    public int StudentID { get; set; }
    public decimal Amount { get; set; }

    /// <summary>UPI | Card | NetBanking | Cash | Cheque</summary>
    public string Method { get; set; } = string.Empty;

    public DateTime PaidAt { get; set; }

    /// <summary>Successful | Failed | Refunded</summary>
    public string Status { get; set; } = string.Empty;
}
