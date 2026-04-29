using System.Net;

namespace EduFlow.Shared.Errors;

/// <summary>
/// Throw when a requested resource doesn't exist.
///
/// Examples:
/// <code>
/// // Generic
/// throw new NotFoundException("USER_NOT_FOUND", "User", id);
///
/// // Inside a service that knows what kind of thing is missing
/// var student = await _repo.FindAsync(studentId);
/// if (student == null)
///     throw new NotFoundException("STUDENT_NOT_FOUND", "Student", studentId);
/// </code>
///
/// Becomes:
/// <code>
/// HTTP 404
/// {
///     "status": 404,
///     "title":  "Student not found",
///     "detail": "Student with id 42 does not exist.",
///     "errorCode": "STUDENT_NOT_FOUND",
///     ...
/// }
/// </code>
/// </summary>
public class NotFoundException : EduFlowException
{
    public NotFoundException(string errorCode, string resourceName, object resourceId)
        : base(
            statusCode: HttpStatusCode.NotFound,
            errorCode: errorCode,
            title: $"{resourceName} not found",
            detail: $"{resourceName} with id {resourceId} does not exist.")
    {
    }

    public NotFoundException(string errorCode, string title, string detail)
        : base(HttpStatusCode.NotFound, errorCode, title, detail)
    {
    }
}
