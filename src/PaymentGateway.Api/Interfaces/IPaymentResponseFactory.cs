using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Interfaces;

public interface IPaymentResponseFactory
{
    Payment CreatePaymentEntity(PostPaymentRequest request, PaymentStatus status, string? authorizationCode = null);
}
