using System.ComponentModel.DataAnnotations;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Validation;

public class FutureDateValidationAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null) return false;

        var request = value as PostPaymentRequest;
        if (request == null) return false;

        // Create the expiry date (first day of the month AFTER expiry month)
        // Credit cards expire at the END of the stated month, so they're valid until the first day of the next month
        try
        {
            var expiryDate = new DateTime(request.ExpiryYear, request.ExpiryMonth, 1).AddMonths(1);
            var currentDate = DateTime.Now.Date;
            
            // Card is valid if expiry date is AFTER current date
            return expiryDate > currentDate;
        }
        catch
        {
            return false; // Invalid date combination
        }
    }

    public override string FormatErrorMessage(string name)
    {
        return "The expiry date must be in the future";
    }
}
