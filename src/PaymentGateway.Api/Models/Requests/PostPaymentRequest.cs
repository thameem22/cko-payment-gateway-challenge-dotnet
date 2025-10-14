using System.ComponentModel.DataAnnotations;
using PaymentGateway.Api.Validation;

namespace PaymentGateway.Api.Models.Requests;

[FutureDateValidation]
public class PostPaymentRequest
{
    [Required]
    [RegularExpression(@"^\d{14,19}$", ErrorMessage = "Card number must be between 14-19 digits and contain only numeric characters")]
    public string CardNumber { get; set; } = string.Empty;

    [Required]
    [Range(1, 12, ErrorMessage = "Expiry month must be between 1-12")]
    public int ExpiryMonth { get; set; }

    [Required]
    [Range(2020, 3000, ErrorMessage = "Expiry year must be a valid year")]
    public int ExpiryYear { get; set; }

    [Required]
    [RegularExpression(@"^(USD|EUR|GBP)$", ErrorMessage = "Currency must be one of: USD, EUR, GBP")]
    public string Currency { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Amount must be a positive integer")]
    public int Amount { get; set; }

    [Required]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3-4 digits")]
    public string Cvv { get; set; } = string.Empty;
}