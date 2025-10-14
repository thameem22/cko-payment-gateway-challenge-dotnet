using PaymentGateway.Api.Services;
using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Tests.TestFixtures;

namespace PaymentGateway.Api.Tests.UnitTests.Services;

public class PaymentsRepositoryTests
{
    private readonly PaymentsRepository _repository;

    public PaymentsRepositoryTests()
    {
        _repository = new PaymentsRepository();
    }

    [Fact]
    public async Task AddAsync_StoresPaymentAndReturnsId()
    {
        // Arrange
        var payment = PaymentEntityBuilder.CreateAuthorized().Build();

        // Act
        var result = await _repository.AddAsync(payment);

        // Assert
        Assert.Equal(payment.Id, result);
    }

    [Fact]
    public async Task GetAsync_WithExistingId_ReturnsPayment()
    {
        // Arrange
        var payment = PaymentEntityBuilder.CreateAuthorized().Build();

        await _repository.AddAsync(payment);

        // Act
        var result = await _repository.GetAsync(payment.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(payment.Id, result!.Id);
        Assert.Equal(payment.Status, result.Status);
        Assert.Equal(payment.CardNumberLastFour, result.CardNumberLastFour);
        Assert.Equal(payment.Currency, result.Currency);
        Assert.Equal(payment.Amount, result.Amount);
    }

    [Fact]
    public async Task GetAsync_WithNonExistentId_ThrowsPaymentNotFoundException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<PaymentNotFoundException>(
            () => _repository.GetAsync(nonExistentId)
        );
        
        Assert.Contains(nonExistentId.ToString(), exception.Message);
    }

    [Fact]
    public async Task AddAsync_MultiplePayments_AllStoredIndependently()
    {
        // Arrange
        var payment1 = PaymentEntityBuilder.CreateAuthorized().Build();
        var payment2 = PaymentEntityBuilder.CreateDeclined()
            .WithCardNumberLastFour("5678")
            .WithExpiryMonth(6)
            .WithExpiryYear(2026)
            .WithCurrency("EUR")
            .WithAmount(2000)
            .Build();

        // Act
        await _repository.AddAsync(payment1);
        await _repository.AddAsync(payment2);

        var result1 = await _repository.GetAsync(payment1.Id);
        var result2 = await _repository.GetAsync(payment2.Id);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(payment1.Id, result1!.Id);
        Assert.Equal(payment2.Id, result2!.Id);
        Assert.NotEqual(result1.Id, result2.Id);
    }
}
