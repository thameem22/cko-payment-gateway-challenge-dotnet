using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Interfaces;

public interface IBankService
{
    Task<BankResponse?> ProcessPaymentAsync(BankRequest bankRequest);
}

