using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Entities;

namespace PaymentGateway.Api.Models.Responses;

public class PostPaymentResponse
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public int CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int Amount { get; set; }

    public static PostPaymentResponse FromEntity(Payment payment)
    {
        return new PostPaymentResponse
        {
            Id = payment.Id,
            Status = payment.Status,
            CardNumberLastFour = int.Parse(payment.CardNumberLastFour),
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount
        };
    }
}
