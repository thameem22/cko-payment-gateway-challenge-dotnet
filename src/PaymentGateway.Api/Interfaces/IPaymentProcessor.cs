using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Interfaces;

public interface IPaymentProcessor
{
    Task<PostPaymentResponse> ProcessPaymentAsync(PostPaymentRequest request);
    Task<GetPaymentResponse> GetPaymentAsync(Guid id);
}
