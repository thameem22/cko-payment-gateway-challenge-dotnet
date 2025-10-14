using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Models.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public string CardNumberLastFour { get; set; } = string.Empty;
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string? AuthorizationCode { get; set; }
    public DateTime CreatedAt { get; set; }
}
