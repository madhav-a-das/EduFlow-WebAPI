namespace EduFlow.Shared.Contracts;

/// <summary>
/// Public shape of an Enrolment as returned by StudentService (M2).
///
/// Spelling note: 'Enrolment' (single L) per the EduFlow design document.
/// The .NET ecosystem more commonly uses 'Enrollment', but we follow the
/// spec to keep DB column names, API routes, and DTOs aligned. This is
/// locked in — do not change it without a project-wide rename.
/// </summary>
public class EnrolmentDto
{
    public int EnrollID { get; set; }
    public int StudentID { get; set; }
    public int CourseID { get; set; }

    /// <summary>e.g. "Spring2026", "Fall2025"</summary>
    public string Semester { get; set; } = string.Empty;

    /// <summary>Active | Dropped | Completed</summary>
    public string Status { get; set; } = string.Empty;

    public DateTime EnrolledAt { get; set; }

    /// <summary>FK to Grade once grading is complete; null while in progress.</summary>
    public int? GradeID { get; set; }
}
