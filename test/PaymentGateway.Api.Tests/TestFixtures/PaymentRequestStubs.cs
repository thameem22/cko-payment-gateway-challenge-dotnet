using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Tests.TestFixtures;

/// <summary>
/// Pre-defined payment request stubs for common test scenarios.
/// Use these for simple validation tests where you don't need customization.
/// For complex scenarios, use PaymentRequestBuilder.
/// </summary>
public static class PaymentRequestStubs
{
    /// <summary>
    /// Valid request that will be authorized by the bank (card ends in 1)
    /// </summary>
    public static PostPaymentRequest ValidAuthorizedCard => new()
    {
        CardNumber = "2222405343248111",
        ExpiryMonth = 12,
        ExpiryYear = 2026,
        Currency = "USD",
        Amount = 1000,
        Cvv = "123"
    };
    
    /// <summary>
    /// Valid request that will be declined by the bank (card ends in 2)
    /// </summary>
    public static PostPaymentRequest ValidDeclinedCard => new()
    {
        CardNumber = "4111111111111112",
        ExpiryMonth = 12,
        ExpiryYear = 2026,
        Currency = "GBP",
        Amount = 2000,
        Cvv = "456"
    };
    
    /// <summary>
    /// Invalid card number (too short)
    /// </summary>
    public static PostPaymentRequest InvalidCardNumberTooShort => new()
    {
        CardNumber = "123",
        ExpiryMonth = 12,
        ExpiryYear = 2026,
        Currency = "USD",
        Amount = 1000,
        Cvv = "123"
    };
    
    /// <summary>
    /// Invalid card number (non-numeric)
    /// </summary>
    public static PostPaymentRequest InvalidCardNumberNonNumeric => new()
    {
        CardNumber = "ABCD1234EFGH5678",
        ExpiryMonth = 12,
        ExpiryYear = 2026,
        Currency = "USD",
        Amount = 1000,
        Cvv = "123"
    };
    
    /// <summary>
    /// Expired card
    /// </summary>
    public static PostPaymentRequest ExpiredCard => new()
    {
        CardNumber = "2222405343248111",
        ExpiryMonth = 12,
        ExpiryYear = 2020,
        Currency = "USD",
        Amount = 1000,
        Cvv = "123"
    };
    
    /// <summary>
    /// Invalid currency code
    /// </summary>
    public static PostPaymentRequest InvalidCurrency => new()
    {
        CardNumber = "2222405343248111",
        ExpiryMonth = 12,
        ExpiryYear = 2026,
        Currency = "XXX",
        Amount = 1000,
        Cvv = "123"
    };
    
    /// <summary>
    /// Invalid CVV (too short)
    /// </summary>
    public static PostPaymentRequest InvalidCvvTooShort => new()
    {
        CardNumber = "2222405343248111",
        ExpiryMonth = 12,
        ExpiryYear = 2026,
        Currency = "USD",
        Amount = 1000,
        Cvv = "12"
    };
    
    /// <summary>
    /// Invalid amount (negative)
    /// </summary>
    public static PostPaymentRequest InvalidAmountNegative => new()
    {
        CardNumber = "2222405343248111",
        ExpiryMonth = 12,
        ExpiryYear = 2026,
        Currency = "USD",
        Amount = -100,
        Cvv = "123"
    };
}



