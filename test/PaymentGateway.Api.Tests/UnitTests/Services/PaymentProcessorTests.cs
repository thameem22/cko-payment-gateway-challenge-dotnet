using PaymentGateway.Api.Services;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Tests.TestFixtures;
using Microsoft.Extensions.Logging;
using Moq;

namespace PaymentGateway.Api.Tests.UnitTests.Services;

public class PaymentProcessorTests
{
    private readonly Mock<IBankService> _mockBankService;
    private readonly Mock<IPaymentResponseFactory> _mockResponseFactory;
    private readonly Mock<IPaymentsRepository> _mockRepository;
    private readonly Mock<ILogger<PaymentProcessor>> _mockLogger;
    private readonly PaymentProcessor _processor;

    public PaymentProcessorTests()
    {
        _mockBankService = new Mock<IBankService>();
        _mockResponseFactory = new Mock<IPaymentResponseFactory>();
        _mockRepository = new Mock<IPaymentsRepository>();
        _mockLogger = new Mock<ILogger<PaymentProcessor>>();
        
        _processor = new PaymentProcessor(
            _mockBankService.Object,
            _mockResponseFactory.Object,
            _mockRepository.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task ProcessPaymentAsync_WhenBankAuthorizes_ReturnsAuthorizedPayment()
    {
        // Arrange
        var request = PaymentRequestBuilder.CreateDefault()
            .WithAuthorizedCard()
            .Build();

        var bankResponse = BankResponseBuilder.CreateAuthorized().Build();

        var expectedEntity = PaymentEntityBuilder.CreateAuthorized().Build();

        _mockBankService.Setup(b => b.ProcessPaymentAsync(It.IsAny<BankRequest>()))
            .ReturnsAsync(bankResponse);

        _mockResponseFactory.Setup(f => f.CreatePaymentEntity(
                It.IsAny<PostPaymentRequest>(),
                PaymentStatus.Authorized,
                "AUTH123"))
            .Returns(expectedEntity);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Payment>()))
            .ReturnsAsync(expectedEntity.Id);

        // Act
        var result = await _processor.ProcessPaymentAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Authorized, result.Status);
        Assert.Equal(expectedEntity.Id, result.Id);
        
        _mockBankService.Verify(b => b.ProcessPaymentAsync(It.Is<BankRequest>(
            r => r.CardNumber == request.CardNumber &&
                 r.Currency == request.Currency &&
                 r.Amount == request.Amount)), Times.Once);
        
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Payment>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WhenBankDeclines_ReturnsDeclinedPayment()
    {
        // Arrange
        var request = PaymentRequestBuilder.CreateDefault()
            .WithDeclinedCard()
            .Build();

        var bankResponse = BankResponseBuilder.CreateDeclined().Build();

        var expectedEntity = PaymentEntityBuilder.CreateDeclined().Build();

        _mockBankService.Setup(b => b.ProcessPaymentAsync(It.IsAny<BankRequest>()))
            .ReturnsAsync(bankResponse);

        _mockResponseFactory.Setup(f => f.CreatePaymentEntity(
                It.IsAny<PostPaymentRequest>(),
                PaymentStatus.Declined,
                null))
            .Returns(expectedEntity);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Payment>()))
            .ReturnsAsync(expectedEntity.Id);

        // Act
        var result = await _processor.ProcessPaymentAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Declined, result.Status);
        
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Payment>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WhenBankReturnsNull_ReturnsDeclined()
    {
        // Arrange
        var request = PaymentRequestBuilder.CreateDefault().Build();

        var expectedEntity = PaymentEntityBuilder.CreateDeclined()
            .WithCardNumberLastFour("3456")
            .Build();

        _mockBankService.Setup(b => b.ProcessPaymentAsync(It.IsAny<BankRequest>()))
            .ReturnsAsync((BankResponse?)null);

        _mockResponseFactory.Setup(f => f.CreatePaymentEntity(
                It.IsAny<PostPaymentRequest>(),
                PaymentStatus.Declined,
                null))
            .Returns(expectedEntity);

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Payment>()))
            .ReturnsAsync(expectedEntity.Id);

        // Act
        var result = await _processor.ProcessPaymentAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PaymentStatus.Declined, result.Status);
        
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Payment>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WhenExceptionOccurs_ThrowsPaymentProcessingException()
    {
        // Arrange
        var request = PaymentRequestBuilder.CreateDefault().Build();

        _mockBankService.Setup(b => b.ProcessPaymentAsync(It.IsAny<BankRequest>()))
            .ThrowsAsync(new Exception("Bank service error"));

        // Act & Assert
        await Assert.ThrowsAsync<PaymentProcessingException>(
            () => _processor.ProcessPaymentAsync(request)
        );
    }
}
