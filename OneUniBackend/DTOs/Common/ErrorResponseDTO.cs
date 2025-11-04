namespace OneUniBackend.DTOs.Common;

public class ErrorResponseDTO
{
    public string Message { get; set; } = null!;
    public List<string>? Errors { get; set; }
    public string? TraceId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ErrorResponseDTO FromMessage(string message, string? traceId = null)
    {
        return new ErrorResponseDTO
        {
            Message = message,
            TraceId = traceId,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ErrorResponseDTO FromErrors(List<string> errors, string? traceId = null)
    {
        return new ErrorResponseDTO
        {
            Message = "Validation failed",
            Errors = errors,
            TraceId = traceId,
            Timestamp = DateTime.UtcNow
        };
    }
}

