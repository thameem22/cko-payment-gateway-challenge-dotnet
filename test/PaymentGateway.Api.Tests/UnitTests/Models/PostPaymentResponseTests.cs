using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Tests.TestFixtures;

namespace PaymentGateway.Api.Tests.UnitTests.Models;

public class PostPaymentResponseTests
{
    [Fact]
    public void FromEntity_MapsAllFieldsCorrectly()
    {
        var entity = PaymentEntityBuilder.CreateAuthorized()
            .WithId(Guid.NewGuid())
            .WithCardNumberLastFour("8877")
            .WithExpiryMonth(12)
            .WithExpiryYear(2026)
            .WithCurrency("GBP")
            .WithAmount(10000)
            .WithStatus(PaymentStatus.Authorized)
            .WithAuthorizationCode("AUTH123")
            .Build();

        var response = PostPaymentResponse.FromEntity(entity);

        Assert.NotNull(response);
        Assert.Equal(entity.Id, response.Id);
        Assert.Equal(entity.Status, response.Status);
        Assert.Equal(8877, response.CardNumberLastFour);
        Assert.Equal(entity.ExpiryMonth, response.ExpiryMonth);
        Assert.Equal(entity.ExpiryYear, response.ExpiryYear);
        Assert.Equal(entity.Currency, response.Currency);
        Assert.Equal(entity.Amount, response.Amount);
    }

    [Fact]
    public void FromEntity_WithDeclinedStatus_MapsCorrectly()
    {
        var entity = PaymentEntityBuilder.CreateDeclined()
            .WithId(Guid.NewGuid())
            .WithCardNumberLastFour("1234")
            .WithStatus(PaymentStatus.Declined)
            .Build();

        var response = PostPaymentResponse.FromEntity(entity);

        Assert.Equal(PaymentStatus.Declined, response.Status);
        Assert.Equal(1234, response.CardNumberLastFour);
    }

    [Fact]
    public void FromEntity_ConvertsStringCardNumberToInt()
    {
        var entity = PaymentEntityBuilder.CreateDefault()
            .WithCardNumberLastFour("0123")
            .Build();

        var response = PostPaymentResponse.FromEntity(entity);

        Assert.Equal(123, response.CardNumberLastFour);
    }

    [Fact]
    public void FromEntity_WithFourDigitCard_MapsCorrectly()
    {
        var entity = PaymentEntityBuilder.CreateDefault()
            .WithCardNumberLastFour("9999")
            .Build();

        var response = PostPaymentResponse.FromEntity(entity);

        Assert.Equal(9999, response.CardNumberLastFour);
    }

    [Fact]
    public void FromEntity_PreservesAllCurrencyCodes()
    {
        var currencies = new[] { "USD", "GBP", "EUR" };

        foreach (var currency in currencies)
        {
            var entity = PaymentEntityBuilder.CreateDefault()
                .WithCurrency(currency)
                .Build();

            var response = PostPaymentResponse.FromEntity(entity);

            Assert.Equal(currency, response.Currency);
        }
    }

    [Fact]
    public void FromEntity_PreservesAmount()
    {
        var amounts = new[] { 1, 100, 1000, 10000, 999999 };

        foreach (var amount in amounts)
        {
            var entity = PaymentEntityBuilder.CreateDefault()
                .WithAmount(amount)
                .Build();

            var response = PostPaymentResponse.FromEntity(entity);

            Assert.Equal(amount, response.Amount);
        }
    }
}



