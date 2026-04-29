using System.Net;

namespace EduFlow.Shared.Errors;

/// <summary>
/// Throw when input validation fails. Optionally include per-field error
/// messages so the frontend can highlight individual form fields.
///
/// Examples:
/// <code>
/// // Single message
/// if (string.IsNullOrEmpty(dto.Email))
///     throw new ValidationException("EMAIL_REQUIRED", "Email is required.");
///
/// // Multiple field errors
/// var errors = new Dictionary&lt;string, string[]&gt;
/// {
///     { "email",    new[] { "Email is required" } },
///     { "password", new[] { "Min 8 chars", "Must contain a digit" } }
/// };
/// throw new ValidationException("VALIDATION_FAILED",
///     "One or more fields are invalid.", errors);
/// </code>
/// </summary>
public class ValidationException : EduFlowException
{
    public Dictionary<string, string[]>? FieldErrors { get; }

    public ValidationException(string errorCode, string detail)
        : base(HttpStatusCode.BadRequest, errorCode, "Validation failed", detail)
    {
    }

    public ValidationException(
        string errorCode,
        string detail,
        Dictionary<string, string[]> fieldErrors)
        : base(HttpStatusCode.BadRequest, errorCode, "Validation failed", detail)
    {
        FieldErrors = fieldErrors;
    }
}
