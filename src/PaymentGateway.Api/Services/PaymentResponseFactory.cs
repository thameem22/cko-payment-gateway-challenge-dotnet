using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Interfaces;

namespace PaymentGateway.Api.Services;

public class PaymentResponseFactory : IPaymentResponseFactory
{
    public Payment CreatePaymentEntity(PostPaymentRequest request, PaymentStatus status, string? authorizationCode = null)
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            CardNumberLastFour = request.CardNumber.Substring(request.CardNumber.Length - 4),
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency,
            Amount = request.Amount,
            Status = status,
            AuthorizationCode = authorizationCode,
            CreatedAt = DateTime.UtcNow
        };
    }
}
