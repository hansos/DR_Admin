using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages payment processing operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentProcessingService _paymentProcessingService;
    private static readonly Serilog.ILogger _log = Log.ForContext<PaymentsController>();

    public PaymentsController(IPaymentProcessingService paymentProcessingService)
    {
        _paymentProcessingService = paymentProcessingService;
    }

    /// <summary>
    /// Processes an invoice payment
    /// </summary>
    [HttpPost("process")]
    [Authorize(Roles = "Admin,Customer")]
    [ProducesResponseType(typeof(PaymentResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentResultDto>> ProcessInvoicePayment([FromBody] ProcessInvoicePaymentDto dto)
    {
        try
        {
            _log.Information("API: ProcessInvoicePayment called for invoice: {InvoiceId}", dto.InvoiceId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get client IP
            dto.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            dto.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var result = await _paymentProcessingService.ProcessInvoicePaymentAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error processing invoice payment");
            return StatusCode(500, "An error occurred while processing the payment");
        }
    }

    /// <summary>
    /// Applies customer credit to an invoice
    /// </summary>
    [HttpPost("apply-credit")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(PaymentResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentResultDto>> ApplyCustomerCredit([FromBody] ApplyCustomerCreditDto dto)
    {
        try
        {
            _log.Information("API: ApplyCustomerCredit called for invoice: {InvoiceId}", dto.InvoiceId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentProcessingService.ApplyCustomerCreditAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error applying customer credit");
            return StatusCode(500, "An error occurred while applying credit");
        }
    }

    /// <summary>
    /// Processes a partial payment
    /// </summary>
    [HttpPost("partial")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(PaymentResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentResultDto>> ProcessPartialPayment([FromBody] ProcessPartialPaymentDto dto)
    {
        try
        {
            _log.Information("API: ProcessPartialPayment called for invoice: {InvoiceId}", dto.InvoiceId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dto.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            dto.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var result = await _paymentProcessingService.ProcessPartialPaymentAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error processing partial payment");
            return StatusCode(500, "An error occurred while processing the partial payment");
        }
    }

    /// <summary>
    /// Retries a failed payment
    /// </summary>
    [HttpPost("retry/{paymentAttemptId}")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(PaymentResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentResultDto>> RetryFailedPayment(int paymentAttemptId)
    {
        try
        {
            _log.Information("API: RetryFailedPayment called for attempt: {PaymentAttemptId}", paymentAttemptId);

            var result = await _paymentProcessingService.RetryFailedPaymentAsync(paymentAttemptId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error retrying payment");
            return StatusCode(500, "An error occurred while retrying the payment");
        }
    }

    /// <summary>
    /// Confirms payment authentication (3D Secure)
    /// </summary>
    [HttpPost("confirm-authentication/{paymentAttemptId}")]
    [Authorize(Roles = "Admin,Customer")]
    [ProducesResponseType(typeof(PaymentResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaymentResultDto>> ConfirmAuthentication(int paymentAttemptId)
    {
        try
        {
            _log.Information("API: ConfirmAuthentication called for attempt: {PaymentAttemptId}", paymentAttemptId);

            var result = await _paymentProcessingService.ConfirmAuthenticationAsync(paymentAttemptId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error confirming authentication");
            return StatusCode(500, "An error occurred while confirming authentication");
        }
    }

    /// <summary>
    /// Gets payment attempts for an invoice
    /// </summary>
    [HttpGet("attempts/invoice/{invoiceId}")]
    [Authorize(Roles = "Admin,Finance,Support")]
    [ProducesResponseType(typeof(IEnumerable<PaymentAttemptDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentAttemptDto>>> GetPaymentAttemptsByInvoiceId(int invoiceId)
    {
        try
        {
            _log.Information("API: GetPaymentAttemptsByInvoiceId called for invoice: {InvoiceId}", invoiceId);

            var attempts = await _paymentProcessingService.GetPaymentAttemptsByInvoiceIdAsync(invoiceId);
            return Ok(attempts);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error retrieving payment attempts");
            return StatusCode(500, "An error occurred while retrieving payment attempts");
        }
    }

    /// <summary>
    /// Gets a payment attempt by ID
    /// </summary>
    [HttpGet("attempts/{id}")]
    [Authorize(Roles = "Admin,Finance,Support")]
    [ProducesResponseType(typeof(PaymentAttemptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentAttemptDto>> GetPaymentAttemptById(int id)
    {
        try
        {
            _log.Information("API: GetPaymentAttemptById called for ID: {Id}", id);

            var attempt = await _paymentProcessingService.GetPaymentAttemptByIdAsync(id);
            if (attempt == null)
                return NotFound($"Payment attempt with ID {id} not found");

            return Ok(attempt);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error retrieving payment attempt");
            return StatusCode(500, "An error occurred while retrieving the payment attempt");
        }
    }

    /// <summary>
    /// Handles payment gateway webhooks
    /// </summary>
    [HttpPost("webhook/{gatewayName}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> HandleWebhook(string gatewayName)
    {
        try
        {
            _log.Information("API: HandleWebhook called for gateway: {GatewayName}", gatewayName);

            using var reader = new StreamReader(HttpContext.Request.Body);
            var payload = await reader.ReadToEndAsync();
            var signature = HttpContext.Request.Headers["Stripe-Signature"].ToString();

            var success = await _paymentProcessingService.HandlePaymentWebhookAsync(gatewayName, payload, signature);

            return success ? Ok() : StatusCode(500);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error handling webhook");
            return StatusCode(500);
        }
    }
}
