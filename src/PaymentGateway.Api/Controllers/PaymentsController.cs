using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Interfaces;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentProcessor _paymentProcessor;

    public PaymentsController(IPaymentProcessor paymentProcessor)
    {
        _paymentProcessor = paymentProcessor;
    }

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse>> PostPaymentAsync([FromBody] PostPaymentRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var payment = await _paymentProcessor.ProcessPaymentAsync(request);
        return Ok(payment);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetPaymentResponse>> GetPaymentAsync(Guid id)
    {
        var response = await _paymentProcessor.GetPaymentAsync(id);
        return Ok(response);
    }
}