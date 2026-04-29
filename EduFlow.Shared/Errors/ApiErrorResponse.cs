using System.Text.Json.Serialization;

namespace EduFlow.Shared.Errors;

/// <summary>
/// The single error response shape used by every EduFlow service.
/// Inspired by RFC 7807 (Problem Details for HTTP APIs) but with an explicit
/// machine-readable ErrorCode for frontend logic and an Errors dictionary for
/// field-level validation feedback.
///
/// Every 4xx and 5xx response from any EduFlow service serializes to this type.
/// Frontend (Angular) can rely on a stable shape and switch on ErrorCode.
///
/// Example output:
/// <code>
/// {
///   "status":    404,
///   "title":     "Student not found",
///   "detail":    "Student with id 999 does not exist.",
///   "errorCode": "STUDENT_NOT_FOUND",
///   "instance":  "/api/student/999",
///   "traceId":   "00-0af7651916cd43dd8448eb211c80319c-...",
///   "errors":    null
/// }
/// </code>
/// </summary>
public class ApiErrorResponse
{
    /// <summary>HTTP status code (400, 401, 403, 404, 409, 500, 503...).</summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }

    /// <summary>Short human-readable summary, e.g. "Student not found".</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Longer human-readable explanation. May be null in production for 500s.</summary>
    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    /// <summary>Machine-readable code in UPPER_SNAKE_CASE, e.g. "STUDENT_NOT_FOUND".</summary>
    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>The URL path that produced this error.</summary>
    [JsonPropertyName("instance")]
    public string? Instance { get; set; }

    /// <summary>Correlation ID — matches the TraceId in Serilog logs for this request.</summary>
    [JsonPropertyName("traceId")]
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    /// Field-level validation errors keyed by field name.
    /// Only populated when ErrorCode = "VALIDATION_FAILED".
    /// Example: { "email": ["Email is required"], "password": ["Min 8 chars"] }
    /// </summary>
    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string[]>? Errors { get; set; }
}
