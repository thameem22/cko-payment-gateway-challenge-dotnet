using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Exceptions;

namespace PaymentGateway.Api.Services;

public class PaymentsRepository : IPaymentsRepository
{
    private readonly List<Payment> _payments = new();
    
    public Task<Guid> AddAsync(Payment payment)
    {
        _payments.Add(payment);
        return Task.FromResult(payment.Id);
    }

    public Task<Payment?> GetAsync(Guid id)
    {
        var payment = _payments.FirstOrDefault(p => p.Id == id);
        if (payment == null)
        {
            throw new PaymentNotFoundException($"Payment with ID {id} was not found");
        }
        return Task.FromResult<Payment?>(payment);
    }
}