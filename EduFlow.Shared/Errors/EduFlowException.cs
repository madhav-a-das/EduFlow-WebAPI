using System.Net;

namespace EduFlow.Shared.Errors;

/// <summary>
/// Base class for all EduFlow domain exceptions. Carries a status code and
/// a UPPER_SNAKE_CASE error code so the middleware can build a complete
/// ApiErrorResponse without needing a switch on derived types.
///
/// Don't throw this directly — throw one of the typed subclasses
/// (NotFoundException, ValidationException, ConflictException, etc.).
/// </summary>
public abstract class EduFlowException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorCode { get; }
    public string Title { get; }

    protected EduFlowException(
        HttpStatusCode statusCode,
        string errorCode,
        string title,
        string detail)
        : base(detail)
    {
        StatusCode = statusCode;
        ErrorCode  = errorCode;
        Title      = title;
    }
}
