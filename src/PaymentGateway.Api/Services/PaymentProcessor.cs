using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Entities;
using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Exceptions;

namespace PaymentGateway.Api.Services;

/// <summary>
/// Payment processor - handles business logic and mapping between DTOs and entities.
/// </summary>
public class PaymentProcessor : IPaymentProcessor
{
    private readonly IBankService _bankService;
    private readonly IPaymentResponseFactory _responseFactory;
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly ILogger<PaymentProcessor> _logger;

    public PaymentProcessor(
        IBankService bankService,
        IPaymentResponseFactory responseFactory,
        IPaymentsRepository paymentsRepository,
        ILogger<PaymentProcessor> logger)
    {
        _bankService = bankService;
        _responseFactory = responseFactory;
        _paymentsRepository = paymentsRepository;
        _logger = logger;
    }

    public async Task<PostPaymentResponse> ProcessPaymentAsync(PostPaymentRequest request)
    {
        try
        {
            var bankRequest = new BankRequest
            {
                CardNumber = request.CardNumber,
                ExpiryDate = $"{request.ExpiryMonth:00}/{request.ExpiryYear}",
                Currency = request.Currency,
                Amount = request.Amount,
                Cvv = request.Cvv
            };

            var bankResponse = await _bankService.ProcessPaymentAsync(bankRequest);
            var status = DeterminePaymentStatus(bankResponse);
            var paymentEntity = _responseFactory.CreatePaymentEntity(request, status, bankResponse?.AuthorizationCode);
            
            await _paymentsRepository.AddAsync(paymentEntity);
            
            _logger.LogInformation("Payment processed with ID: {PaymentId}, Status: {Status}", paymentEntity.Id, paymentEntity.Status);
            
            return PostPaymentResponse.FromEntity(paymentEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment");
            throw new PaymentProcessingException("Payment processing failed due to technical error", ex);
        }
    }

    public async Task<GetPaymentResponse> GetPaymentAsync(Guid id)
    {
        _logger.LogInformation("Retrieving payment with ID: {PaymentId}", id);
        var paymentEntity = await _paymentsRepository.GetAsync(id);
        return GetPaymentResponse.FromEntity(paymentEntity!);
    }

    private static PaymentStatus DeterminePaymentStatus(BankResponse? bankResponse)
    {
        if (bankResponse == null)
            return PaymentStatus.Declined;
            
        return bankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
    }
}
