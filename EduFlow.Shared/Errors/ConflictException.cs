using System.Net;

namespace EduFlow.Shared.Errors;

/// <summary>
/// Throw when an operation conflicts with the current state.
/// Common cases: duplicate email registration, enrolling in a full course,
/// trying to grade an enrolment that's already graded.
///
/// Examples:
/// <code>
/// if (await _repo.EmailExistsAsync(email))
///     throw new ConflictException("EMAIL_ALREADY_EXISTS",
///         $"A user with email {email} already exists.");
///
/// if (course.Enrolments.Count >= course.Capacity)
///     throw new ConflictException("COURSE_FULL",
///         $"Course {course.Code} has reached capacity.");
/// </code>
/// </summary>
public class ConflictException : EduFlowException
{
    public ConflictException(string errorCode, string detail)
        : base(HttpStatusCode.Conflict, errorCode, "Conflict", detail)
    {
    }

    public ConflictException(string errorCode, string title, string detail)
        : base(HttpStatusCode.Conflict, errorCode, title, detail)
    {
    }
}
