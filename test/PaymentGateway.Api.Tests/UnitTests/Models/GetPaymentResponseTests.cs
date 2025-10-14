using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Tests.TestFixtures;

namespace PaymentGateway.Api.Tests.UnitTests.Models;

public class GetPaymentResponseTests
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
            .Build();

        var response = GetPaymentResponse.FromEntity(entity);

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
            .WithStatus(PaymentStatus.Declined)
            .Build();

        var response = GetPaymentResponse.FromEntity(entity);

        Assert.Equal(PaymentStatus.Declined, response.Status);
    }

    [Fact]
    public void FromEntity_ConvertsStringCardNumberToInt()
    {
        var entity = PaymentEntityBuilder.CreateDefault()
            .WithCardNumberLastFour("0001")
            .Build();

        var response = GetPaymentResponse.FromEntity(entity);

        Assert.Equal(1, response.CardNumberLastFour);
    }

    [Fact]
    public void FromEntity_WithLeadingZeros_HandlesCorrectly()
    {
        var testCases = new[]
        {
            ("0001", 1),
            ("0123", 123),
            ("0000", 0),
            ("9999", 9999)
        };

        foreach (var (cardLast4, expected) in testCases)
        {
            var entity = PaymentEntityBuilder.CreateDefault()
                .WithCardNumberLastFour(cardLast4)
                .Build();

            var response = GetPaymentResponse.FromEntity(entity);

            Assert.Equal(expected, response.CardNumberLastFour);
        }
    }

    [Fact]
    public void FromEntity_DoesNotIncludeAuthorizationCode()
    {
        var entity = PaymentEntityBuilder.CreateAuthorized()
            .WithAuthorizationCode("SECRET-AUTH-CODE-123")
            .Build();

        var response = GetPaymentResponse.FromEntity(entity);

        var responseType = response.GetType();
        var authProperty = responseType.GetProperty("AuthorizationCode");
        
        Assert.Null(authProperty);
    }

    [Fact]
    public void FromEntity_DoesNotIncludeCreatedAt()
    {
        var entity = PaymentEntityBuilder.CreateDefault()
            .WithCreatedAt(DateTime.UtcNow.AddDays(-30))
            .Build();

        var response = GetPaymentResponse.FromEntity(entity);

        var responseType = response.GetType();
        var createdAtProperty = responseType.GetProperty("CreatedAt");
        
        Assert.Null(createdAtProperty);
    }
}



