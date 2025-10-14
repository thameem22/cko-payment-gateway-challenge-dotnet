using PaymentGateway.Api.Models;
using PaymentGateway.Api.Interfaces;
using System.Text.Json;

namespace PaymentGateway.Api.Services;

public class BankService : IBankService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BankService> _logger;

    public BankService(IHttpClientFactory httpClientFactory, ILogger<BankService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<BankResponse?> ProcessPaymentAsync(BankRequest bankRequest)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("BankClient");
            
            var json = JsonSerializer.Serialize(bankRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending request to bank: {Request}", json);

            var response = await httpClient.PostAsync("/payments", content);
            
            _logger.LogInformation("Bank response status: {StatusCode}", response.StatusCode);
            
            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                _logger.LogWarning("Bank simulator returned service unavailable");
                return null; // This will result in a Rejected status
            }

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Bank response content: {Content}", responseContent);
                
                var bankResponse = JsonSerializer.Deserialize<BankResponse>(responseContent);
                _logger.LogInformation("Parsed bank response - Authorized: {Authorized}, Code: {Code}", 
                    bankResponse?.Authorized, bankResponse?.AuthorizationCode);
                    
                return bankResponse;
            }
            
            _logger.LogError($"Bank simulator returned error: {response.StatusCode}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling bank simulator");
            return null;
        }
    }
}

