namespace EduFlow.Shared.Contracts;

/// <summary>Returned by AcademicService (M2/M3+M4 in the docs).</summary>
public class CourseDto
{
    public int CourseID { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int DepartmentID { get; set; }

    /// <summary>Active | Archived</summary>
    public string Status { get; set; } = string.Empty;
}
