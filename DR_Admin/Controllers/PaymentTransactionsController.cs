using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Provides read access to payment transaction list data.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PaymentTransactionsController : ControllerBase
{
    private readonly IPaymentTransactionService _paymentTransactionService;
    private static readonly Serilog.ILogger _log = Log.ForContext<PaymentTransactionsController>();

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentTransactionsController"/> class.
    /// </summary>
    /// <param name="paymentTransactionService">The payment transaction service.</param>
    public PaymentTransactionsController(IPaymentTransactionService paymentTransactionService)
    {
        _paymentTransactionService = paymentTransactionService;
    }

    /// <summary>
    /// Retrieves all payment transactions with invoice and allocation details.
    /// </summary>
    /// <returns>Payment transaction list items.</returns>
    /// <response code="200">Returns payment transaction list items.</response>
    /// <response code="401">If user is not authenticated.</response>
    /// <response code="403">If user doesn't have required permission.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet]
    [Authorize(Policy = "PaymentTransaction.Read")]
    [ProducesResponseType(typeof(IEnumerable<PaymentTransactionListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PaymentTransactionListDto>>> GetAll()
    {
        try
        {
            _log.Information("API: GetAll payment transactions called by user {User}", User.Identity?.Name);
            var items = await _paymentTransactionService.GetAllAsync();
            return Ok(items);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error getting payment transactions list");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving payment transactions");
        }
    }
}
