using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Entities;

namespace PaymentGateway.Api.Tests.TestFixtures;

/// <summary>
/// Builder for creating Payment entities for testing.
/// </summary>
public class PaymentEntityBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _cardNumberLastFour = "8877";
    private int _expiryMonth = 12;
    private int _expiryYear = 2026;
    private string _currency = "GBP";
    private int _amount = 10000;
    private PaymentStatus _status = PaymentStatus.Authorized;
    private string? _authorizationCode = null;
    private DateTime _createdAt = DateTime.UtcNow;

    public static PaymentEntityBuilder CreateDefault() => new();

    public static PaymentEntityBuilder CreateAuthorized()
    {
        return new PaymentEntityBuilder()
            .WithStatus(PaymentStatus.Authorized)
            .WithAuthorizationCode("auth-code-123");
    }

    public static PaymentEntityBuilder CreateDeclined()
    {
        return new PaymentEntityBuilder()
            .WithStatus(PaymentStatus.Declined)
            .WithAuthorizationCode(null);
    }

    public PaymentEntityBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public PaymentEntityBuilder WithCardNumberLastFour(string lastFour)
    {
        _cardNumberLastFour = lastFour;
        return this;
    }

    public PaymentEntityBuilder WithExpiryMonth(int month)
    {
        _expiryMonth = month;
        return this;
    }

    public PaymentEntityBuilder WithExpiryYear(int year)
    {
        _expiryYear = year;
        return this;
    }

    public PaymentEntityBuilder WithCurrency(string currency)
    {
        _currency = currency;
        return this;
    }

    public PaymentEntityBuilder WithAmount(int amount)
    {
        _amount = amount;
        return this;
    }

    public PaymentEntityBuilder WithStatus(PaymentStatus status)
    {
        _status = status;
        return this;
    }

    public PaymentEntityBuilder WithAuthorizationCode(string? code)
    {
        _authorizationCode = code;
        return this;
    }

    public PaymentEntityBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public Payment Build()
    {
        return new Payment
        {
            Id = _id,
            CardNumberLastFour = _cardNumberLastFour,
            ExpiryMonth = _expiryMonth,
            ExpiryYear = _expiryYear,
            Currency = _currency,
            Amount = _amount,
            Status = _status,
            AuthorizationCode = _authorizationCode,
            CreatedAt = _createdAt
        };
    }
}



