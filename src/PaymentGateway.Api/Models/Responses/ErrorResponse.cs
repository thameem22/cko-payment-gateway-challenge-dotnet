namespace PaymentGateway.Api.Models.Responses;

public class ErrorResponse
{
    public string Message { get; }
    public string? Details { get; }
    public DateTime Timestamp { get; }

    public ErrorResponse(string message, string? details = null)
    {
        Message = message;
        Details = details;
        Timestamp = DateTime.UtcNow;
    }
}
