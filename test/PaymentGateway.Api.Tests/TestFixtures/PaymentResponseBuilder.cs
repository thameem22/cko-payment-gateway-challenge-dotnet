using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Tests.TestFixtures;

public class PaymentResponseBuilder
{
    private Guid _id = Guid.NewGuid();
    private PaymentStatus _status = PaymentStatus.Authorized;
    private int _cardNumberLastFour = 3456;
    private int _expiryMonth = 12;
    private int _expiryYear = 2025;
    private string _currency = "USD";
    private int _amount = 1000;

    public static PaymentResponseBuilder CreateDefault() => new();

    public static PaymentResponseBuilder CreateAuthorized() => 
        new PaymentResponseBuilder().WithStatus(PaymentStatus.Authorized);

    public static PaymentResponseBuilder CreateDeclined() => 
        new PaymentResponseBuilder().WithStatus(PaymentStatus.Declined);

    public PaymentResponseBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public PaymentResponseBuilder WithStatus(PaymentStatus status)
    {
        _status = status;
        return this;
    }

    public PaymentResponseBuilder WithCardNumberLastFour(int lastFour)
    {
        _cardNumberLastFour = lastFour;
        return this;
    }

    public PaymentResponseBuilder WithExpiryMonth(int month)
    {
        _expiryMonth = month;
        return this;
    }

    public PaymentResponseBuilder WithExpiryYear(int year)
    {
        _expiryYear = year;
        return this;
    }

    public PaymentResponseBuilder WithCurrency(string currency)
    {
        _currency = currency;
        return this;
    }

    public PaymentResponseBuilder WithAmount(int amount)
    {
        _amount = amount;
        return this;
    }

    public PostPaymentResponse Build()
    {
        return new PostPaymentResponse
        {
            Id = _id,
            Status = _status,
            CardNumberLastFour = _cardNumberLastFour,
            ExpiryMonth = _expiryMonth,
            ExpiryYear = _expiryYear,
            Currency = _currency,
            Amount = _amount
        };
    }
}


