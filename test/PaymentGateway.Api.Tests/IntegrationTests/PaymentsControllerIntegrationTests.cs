using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Tests.TestFixtures;

namespace PaymentGateway.Api.Tests.IntegrationTests;

public class PaymentsControllerIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public PaymentsControllerIntegrationTests(IntegrationTestFixture fixture)
    {
        _client = fixture.Client;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        var response = await _client.GetAsync(ApiEndpoints.GetPaymentById(Guid.NewGuid()));
        
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ProcessPayment_WithValidAuthorizedCard_ReturnsAuthorized()
    {
        var request = PaymentRequestBuilder.CreateDefault()
            .WithCardNumber("2222405343248111")
            .WithCurrency("USD")
            .WithAmount(5000)
            .WithExpiryMonth(12)
            .WithExpiryYear(2026)
            .WithCvv("123")
            .Build();

        var response = await _client.PostAsJsonAsync(ApiEndpoints.Payments, request);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(PaymentStatus.Authorized, paymentResponse!.Status);
        Assert.Equal(8111, paymentResponse.CardNumberLastFour);
        Assert.Equal(12, paymentResponse.ExpiryMonth);
        Assert.Equal(2026, paymentResponse.ExpiryYear);
        Assert.Equal("USD", paymentResponse.Currency);
        Assert.Equal(5000, paymentResponse.Amount);
        Assert.NotEqual(Guid.Empty, paymentResponse.Id);
    }

    [Fact]
    public async Task ProcessPayment_WithDeclinedCard_ReturnsDeclined()
    {
        var request = PaymentRequestBuilder.CreateDefault()
            .WithCardNumber("4111111111111112")
            .WithCurrency("GBP")
            .WithAmount(10000)
            .WithExpiryMonth(6)
            .WithExpiryYear(2027)
            .Build();

        var response = await _client.PostAsJsonAsync(ApiEndpoints.Payments, request);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(PaymentStatus.Declined, paymentResponse!.Status);
        Assert.Equal(1112, paymentResponse.CardNumberLastFour);
    }

    [Fact]
    public async Task ProcessPayment_WithCardEndingInZero_ReturnsDeclined()
    {
        var request = PaymentRequestBuilder.CreateDefault()
            .WithCardNumber("4111111111111110")
            .WithCurrency("GBP")
            .WithAmount(5000)
            .WithExpiryMonth(6)
            .WithExpiryYear(2027)
            .Build();

        var response = await _client.PostAsJsonAsync(ApiEndpoints.Payments, request);
        var paymentResponse = await response.Content.ReadFromJsonAsync<PostPaymentResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        Assert.Equal(PaymentStatus.Declined, paymentResponse!.Status);
        Assert.Equal(1110, paymentResponse.CardNumberLastFour);
    }

    [Fact]
    public async Task ProcessPayment_ThenRetrieve_StoresInRealRepository()
    {
        var request = PaymentRequestBuilder.CreateDefault()
            .WithCardNumber("5555555555554443")
            .WithCurrency("EUR")
            .WithAmount(7500)
            .WithExpiryMonth(3)
            .WithExpiryYear(2028)
            .Build();

        var processResponse = await _client.PostAsJsonAsync(ApiEndpoints.Payments, request);
        var processedPayment = await processResponse.Content.ReadFromJsonAsync<PostPaymentResponse>(_jsonOptions);
        
        Assert.NotNull(processedPayment);

        var retrieveResponse = await _client.GetAsync(ApiEndpoints.GetPaymentById(processedPayment!.Id));
        var retrievedPayment = await retrieveResponse.Content.ReadFromJsonAsync<GetPaymentResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, retrieveResponse.StatusCode);
        Assert.NotNull(retrievedPayment);
        Assert.Equal(processedPayment.Id, retrievedPayment!.Id);
        Assert.Equal(processedPayment.Status, retrievedPayment.Status);
        Assert.Equal(processedPayment.CardNumberLastFour, retrievedPayment.CardNumberLastFour);
        Assert.Equal(processedPayment.ExpiryMonth, retrievedPayment.ExpiryMonth);
        Assert.Equal(processedPayment.ExpiryYear, retrievedPayment.ExpiryYear);
        Assert.Equal(processedPayment.Currency, retrievedPayment.Currency);
        Assert.Equal(processedPayment.Amount, retrievedPayment.Amount);
    }

    [Fact]
    public async Task ProcessPayment_WithInvalidCardNumber_ReturnsBadRequest()
    {
        var request = PaymentRequestStubs.InvalidCardNumberTooShort;

        var response = await _client.PostAsJsonAsync(ApiEndpoints.Payments, request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ProcessPayment_WithInvalidCurrency_ReturnsBadRequest()
    {
        var request = PaymentRequestStubs.InvalidCurrency;

        var response = await _client.PostAsJsonAsync(ApiEndpoints.Payments, request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ProcessPayment_WithExpiredCard_ReturnsBadRequest()
    {
        var request = PaymentRequestStubs.ExpiredCard;

        var response = await _client.PostAsJsonAsync(ApiEndpoints.Payments, request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ProcessAndRetrieveMultiplePayments_MaintainsDataIntegrity()
    {
        var payment1Request = PaymentRequestBuilder.CreateDefault()
            .WithCardNumber("1111111111111111")
            .WithAmount(1000)
            .WithExpiryMonth(12)
            .WithExpiryYear(2026)
            .Build();
        
        var payment2Request = PaymentRequestBuilder.CreateDefault()
            .WithCardNumber("2222222222222223")
            .WithAmount(2000)
            .WithExpiryMonth(12)
            .WithExpiryYear(2026)
            .Build();

        var response1 = await _client.PostAsJsonAsync(ApiEndpoints.Payments, payment1Request);
        var payment1 = await response1.Content.ReadFromJsonAsync<PostPaymentResponse>(_jsonOptions);
        
        var response2 = await _client.PostAsJsonAsync(ApiEndpoints.Payments, payment2Request);
        var payment2 = await response2.Content.ReadFromJsonAsync<PostPaymentResponse>(_jsonOptions);

        Assert.NotNull(payment1);
        Assert.NotNull(payment2);
        Assert.NotEqual(payment1!.Id, payment2!.Id);

        var retrieve1 = await _client.GetAsync(ApiEndpoints.GetPaymentById(payment1.Id));
        var retrieve2 = await _client.GetAsync(ApiEndpoints.GetPaymentById(payment2.Id));

        Assert.Equal(HttpStatusCode.OK, retrieve1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, retrieve2.StatusCode);

        var retrieved1 = await retrieve1.Content.ReadFromJsonAsync<GetPaymentResponse>(_jsonOptions);
        var retrieved2 = await retrieve2.Content.ReadFromJsonAsync<GetPaymentResponse>(_jsonOptions);

        Assert.NotNull(retrieved1);
        Assert.NotNull(retrieved2);
        Assert.Equal(1000, retrieved1!.Amount);
        Assert.Equal(2000, retrieved2!.Amount);
    }
}
