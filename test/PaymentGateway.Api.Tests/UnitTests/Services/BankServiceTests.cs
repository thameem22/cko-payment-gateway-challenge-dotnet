using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests.UnitTests.Services;

public class BankServiceTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<ILogger<BankService>> _mockLogger;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly BankService _bankService;

    public BankServiceTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLogger = new Mock<ILogger<BankService>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://localhost:8080")
        };
        
        _mockHttpClientFactory.Setup(x => x.CreateClient("BankClient")).Returns(httpClient);
        _bankService = new BankService(_mockHttpClientFactory.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithAuthorizedResponse_ReturnsAuthorizedBankResponse()
    {
        // Arrange
        var bankResponse = new BankResponse { Authorized = true, AuthorizationCode = "AUTH-123" };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(bankResponse), Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _bankService.ProcessPaymentAsync(new BankRequest());

        // Assert
        Assert.NotNull(result);
        Assert.True(result!.Authorized);
        Assert.Equal("AUTH-123", result.AuthorizationCode);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithDeclinedResponse_ReturnsDeclinedBankResponse()
    {
        // Arrange
        var bankResponse = new BankResponse { Authorized = false, AuthorizationCode = "" };
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(bankResponse), Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _bankService.ProcessPaymentAsync(new BankRequest());

        // Assert
        Assert.NotNull(result);
        Assert.False(result!.Authorized);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WithServiceUnavailable_ReturnsNull()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _bankService.ProcessPaymentAsync(new BankRequest());

        // Assert
        Assert.Null(result);
    }
}
