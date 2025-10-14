using PaymentGateway.Api.Services;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Tests.TestFixtures;

namespace PaymentGateway.Api.Tests.UnitTests.Services;

public class PaymentResponseFactoryTests
{
    private readonly PaymentResponseFactory _factory;

    public PaymentResponseFactoryTests()
    {
        _factory = new PaymentResponseFactory();
    }

    [Fact]
    public void CreatePaymentEntity_WithAuthorizedStatus_ReturnsCorrectEntity()
    {
        // Arrange
        var request = PaymentRequestBuilder.CreateDefault().Build();

        // Act
        var result = _factory.CreatePaymentEntity(request, PaymentStatus.Authorized, "AUTH123");

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(PaymentStatus.Authorized, result.Status);
        Assert.Equal("3456", result.CardNumberLastFour);
        Assert.Equal(12, result.ExpiryMonth);
        Assert.Equal(2025, result.ExpiryYear);
        Assert.Equal("USD", result.Currency);
        Assert.Equal(1000, result.Amount);
        Assert.Equal("AUTH123", result.AuthorizationCode);
    }

    [Fact]
    public void CreatePaymentEntity_WithDeclinedStatus_ReturnsCorrectEntity()
    {
        // Arrange
        var request = PaymentRequestBuilder.CreateDefault()
            .WithCardNumber("9876543210987654")
            .WithExpiryMonth(6)
            .WithExpiryYear(2026)
            .WithCurrency("EUR")
            .WithAmount(2000)
            .WithCvv("456")
            .Build();

        // Act
        var result = _factory.CreatePaymentEntity(request, PaymentStatus.Declined);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(PaymentStatus.Declined, result.Status);
        Assert.Equal("7654", result.CardNumberLastFour);
        Assert.Equal(6, result.ExpiryMonth);
        Assert.Equal(2026, result.ExpiryYear);
        Assert.Equal("EUR", result.Currency);
        Assert.Equal(2000, result.Amount);
    }

    [Fact]
    public void CreatePaymentEntity_ExtractsLastFourDigitsCorrectly()
    {
        // Arrange
        var request = PaymentRequestBuilder.CreateDefault()
            .WithCardNumber("4532015112830366")
            .WithExpiryMonth(1)
            .WithExpiryYear(2027)
            .WithAmount(100)
            .Build();

        // Act
        var result = _factory.CreatePaymentEntity(request, PaymentStatus.Authorized);

        // Assert
        Assert.Equal("0366", result.CardNumberLastFour);
    }

    [Fact]
    public void CreatePaymentEntity_GeneratesUniqueIds()
    {
        // Arrange
        var request = PaymentRequestBuilder.CreateDefault().Build();

        // Act
        var result1 = _factory.CreatePaymentEntity(request, PaymentStatus.Authorized);
        var result2 = _factory.CreatePaymentEntity(request, PaymentStatus.Authorized);

        // Assert
        Assert.NotEqual(result1.Id, result2.Id);
    }
}
