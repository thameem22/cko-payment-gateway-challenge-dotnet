namespace PaymentGateway.Api.Tests.TestFixtures;

/// <summary>
/// Centralized API endpoint definitions for tests.
/// Eliminates hardcoded URL strings and makes endpoint changes easier.
/// </summary>
public static class ApiEndpoints
{
    public const string Payments = "/api/Payments";
    
    public static string GetPaymentById(Guid id) => $"{Payments}/{id}";
}



