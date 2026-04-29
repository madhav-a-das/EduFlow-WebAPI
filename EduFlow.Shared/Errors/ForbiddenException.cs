using System.Net;

namespace EduFlow.Shared.Errors;

/// <summary>
/// Throw when a user is authenticated but not allowed to perform an action.
///
/// Note: Most 403s come automatically from [Authorize(Roles="...")]. You
/// only throw this manually for resource-level authorization the framework
/// can't check — e.g. "you can read your OWN grades but not someone else's."
///
/// Examples:
/// <code>
/// // A student trying to read another student's grades
/// if (currentUserId != grade.StudentUserId &amp;&amp; currentRole != "Faculty")
///     throw new ForbiddenException("CROSS_STUDENT_ACCESS",
///         "You may only view your own grades.");
///
/// // Student trying to drop a course past the deadline
/// if (DateTime.UtcNow > course.DropDeadline)
///     throw new ForbiddenException("DROP_DEADLINE_PASSED",
///         "The drop deadline for this course has passed.");
/// </code>
/// </summary>
public class ForbiddenException : EduFlowException
{
    public ForbiddenException(string errorCode, string detail)
        : base(HttpStatusCode.Forbidden, errorCode, "Access denied", detail)
    {
    }
}
