using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Models.Responses;
using System.Net;

namespace PaymentGateway.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        var response = exception switch
        {
            PaymentNotFoundException ex => CreateErrorResponse(
                HttpStatusCode.NotFound, 
                "Payment not found", 
                ex.Message),
            
            PaymentProcessingException ex => CreateErrorResponse(
                HttpStatusCode.BadRequest, 
                "Payment processing failed", 
                ex.Message),
            
            ArgumentException ex => CreateErrorResponse(
                HttpStatusCode.BadRequest, 
                "Invalid argument", 
                ex.Message),
            
            _ => CreateErrorResponse(
                HttpStatusCode.InternalServerError, 
                "An unexpected error occurred", 
                "Please try again later")
        };

        httpContext.Response.StatusCode = (int)response.StatusCode;

        await httpContext.Response.WriteAsJsonAsync(
            response.ErrorResponse, 
            cancellationToken);

        return true;
    }

    private static (HttpStatusCode StatusCode, ErrorResponse ErrorResponse) CreateErrorResponse(
        HttpStatusCode statusCode, 
        string message, 
        string? details = null)
    {
        return (statusCode, new ErrorResponse(message, details));
    }
}
