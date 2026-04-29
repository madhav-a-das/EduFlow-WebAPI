namespace EduFlow.Shared.Contracts;

/// <summary>
/// Returned by GradingService (M6).
/// </summary>
public class GradeDto
{
    public int GradeID { get; set; }
    public int EnrollmentID { get; set; }
    public decimal MarksObtained { get; set; }

    /// <summary>e.g. "A", "B+", "F", "I" (incomplete)</summary>
    public string GradeLetter { get; set; } = string.Empty;

    public string Semester { get; set; } = string.Empty;
    public int RecordedByFK { get; set; }
    public DateTime RecordedAt { get; set; }
}
