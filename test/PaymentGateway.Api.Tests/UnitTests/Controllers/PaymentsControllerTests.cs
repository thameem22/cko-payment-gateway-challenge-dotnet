using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Tests.TestFixtures;
using Moq;

namespace PaymentGateway.Api.Tests.UnitTests.Controllers;

public class PaymentsControllerTests
{
    private readonly Mock<IPaymentProcessor> _mockProcessor;
    private readonly PaymentsController _controller;

    public PaymentsControllerTests()
    {
        _mockProcessor = new Mock<IPaymentProcessor>();
        _controller = new PaymentsController(_mockProcessor.Object);
    }

    [Fact]
    public async Task PostPaymentAsync_WithValidRequest_ReturnsOkWithPaymentResponse()
    {
        // Arrange
        var request = PaymentRequestBuilder.CreateDefault().Build();
        var expectedResponse = PaymentResponseBuilder.CreateAuthorized().Build();

        _mockProcessor.Setup(p => p.ProcessPaymentAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.PostPaymentAsync(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualResponse = Assert.IsType<PostPaymentResponse>(okResult.Value);
        Assert.Equal(expectedResponse.Id, actualResponse.Id);
        Assert.Equal(expectedResponse.Status, actualResponse.Status);
        
        _mockProcessor.Verify(p => p.ProcessPaymentAsync(request), Times.Once);
    }

    [Fact]
    public async Task PostPaymentAsync_WithInvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var request = PaymentRequestBuilder.CreateDefault()
            .WithInvalidCardNumber()
            .Build();

        _controller.ModelState.AddModelError("CardNumber", "Card number must be between 14-19 digits");

        // Act
        var result = await _controller.PostPaymentAsync(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
        
        _mockProcessor.Verify(p => p.ProcessPaymentAsync(It.IsAny<PostPaymentRequest>()), Times.Never);
    }

    [Fact]
    public async Task GetPaymentAsync_WithExistingId_ReturnsOkWithPaymentResponse()
    {
        // Arrange
        var expectedResponse = new GetPaymentResponse
        {
            Id = Guid.NewGuid(),
            Status = PaymentStatus.Authorized,
            CardNumberLastFour = 8877,
            ExpiryMonth = 12,
            ExpiryYear = 2026,
            Currency = "GBP",
            Amount = 10000
        };

        _mockProcessor.Setup(p => p.GetPaymentAsync(expectedResponse.Id))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetPaymentAsync(expectedResponse.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualResponse = Assert.IsType<GetPaymentResponse>(okResult.Value);
        Assert.Equal(expectedResponse.Id, actualResponse.Id);
        Assert.Equal(expectedResponse.Status, actualResponse.Status);
        Assert.Equal(expectedResponse.CardNumberLastFour, actualResponse.CardNumberLastFour);
        Assert.Equal(expectedResponse.Currency, actualResponse.Currency);
        Assert.Equal(expectedResponse.Amount, actualResponse.Amount);
        
        _mockProcessor.Verify(p => p.GetPaymentAsync(expectedResponse.Id), Times.Once);
    }

    [Fact]
    public async Task GetPaymentAsync_WithNonExistentId_ThrowsPaymentNotFoundException()
    {
        // Arrange
        var paymentId = Guid.NewGuid();

        _mockProcessor.Setup(p => p.GetPaymentAsync(paymentId))
            .ThrowsAsync(new PaymentNotFoundException($"Payment with ID {paymentId} was not found"));

        // Act & Assert
        await Assert.ThrowsAsync<PaymentNotFoundException>(
            () => _controller.GetPaymentAsync(paymentId)
        );
        
        _mockProcessor.Verify(p => p.GetPaymentAsync(paymentId), Times.Once);
    }

    [Fact]
    public async Task PostPaymentAsync_ReturnsDeclinedPayment_WhenBankDeclines()
    {
        // Arrange
        var request = PaymentRequestBuilder.CreateDefault()
            .WithDeclinedCard()
            .Build();

        var expectedResponse = PaymentResponseBuilder.CreateDeclined()
            .WithCardNumberLastFour(8877)
            .Build();

        _mockProcessor.Setup(p => p.ProcessPaymentAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.PostPaymentAsync(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualResponse = Assert.IsType<PostPaymentResponse>(okResult.Value);
        Assert.Equal(PaymentStatus.Declined, actualResponse.Status);
    }
}
