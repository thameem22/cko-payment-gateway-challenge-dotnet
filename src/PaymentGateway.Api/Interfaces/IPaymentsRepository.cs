using PaymentGateway.Api.Models.Entities;

namespace PaymentGateway.Api.Interfaces;

public interface IPaymentsRepository
{
    Task<Guid> AddAsync(Payment payment);
    Task<Payment?> GetAsync(Guid id);
}
