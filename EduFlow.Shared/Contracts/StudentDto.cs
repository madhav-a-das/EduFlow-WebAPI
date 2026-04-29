namespace EduFlow.Shared.Contracts;

/// <summary>
/// Public shape of a Student as returned by StudentService (M2) endpoints.
/// Note: M2 does not exist yet. This DTO is here so M3, M4, M6, M8 can
/// already declare clients and types — they will compile against this
/// contract today, and "just work" the day M2 ships matching it.
///
/// IMPORTANT: StudentID is NOT the same as UserID.
///   - UserID  → row in M1's User table (any role)
///   - StudentID → row in M2's Student table (only students)
///   - Student.UserID is a foreign key from Student → User
///
/// When validating an Attendance.StudentID or Invoice.StudentID, validate
/// against StudentService (this DTO), NOT IdentityService.
/// </summary>
public class StudentDto
{
    public int StudentID { get; set; }
    public int UserID { get; set; }
    public string RollNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime DOB { get; set; }
    public string Gender { get; set; } = string.Empty;
    public int DepartmentID { get; set; }

    /// <summary>Active | Suspended | Graduated</summary>
    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
