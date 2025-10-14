using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.TestFixtures;

public class PaymentRequestBuilder
{
    private string _cardNumber = "1234567890123456";
    private int _expiryMonth = 12;
    private int _expiryYear = 2025;
    private string _currency = "USD";
    private int _amount = 1000;
    private string _cvv = "123";

    public static PaymentRequestBuilder CreateDefault() => new();

    public PaymentRequestBuilder WithCardNumber(string cardNumber)
    {
        _cardNumber = cardNumber;
        return this;
    }

    public PaymentRequestBuilder WithAuthorizedCard()
    {
        _cardNumber = "1234567890123451"; // Odd number - authorized
        return this;
    }

    public PaymentRequestBuilder WithDeclinedCard()
    {
        _cardNumber = "2222405343248877"; // Even number - declined
        return this;
    }

    public PaymentRequestBuilder WithInvalidCardNumber()
    {
        _cardNumber = "123"; // Too short
        return this;
    }

    public PaymentRequestBuilder WithExpiryMonth(int month)
    {
        _expiryMonth = month;
        return this;
    }

    public PaymentRequestBuilder WithExpiryYear(int year)
    {
        _expiryYear = year;
        return this;
    }

    public PaymentRequestBuilder WithExpiredDate()
    {
        _expiryMonth = 4;
        _expiryYear = 2020;
        return this;
    }

    public PaymentRequestBuilder WithCurrency(string currency)
    {
        _currency = currency;
        return this;
    }

    public PaymentRequestBuilder WithInvalidCurrency()
    {
        _currency = "INVALID";
        return this;
    }

    public PaymentRequestBuilder WithAmount(int amount)
    {
        _amount = amount;
        return this;
    }

    public PaymentRequestBuilder WithCvv(string cvv)
    {
        _cvv = cvv;
        return this;
    }

    public PostPaymentRequest Build()
    {
        return new PostPaymentRequest
        {
            CardNumber = _cardNumber,
            ExpiryMonth = _expiryMonth,
            ExpiryYear = _expiryYear,
            Currency = _currency,
            Amount = _amount,
            Cvv = _cvv
        };
    }
}


