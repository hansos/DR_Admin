using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages customer credit balances and transactions
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CustomerCreditsController : ControllerBase
{
    private readonly ICreditService _creditService;
    private static readonly Serilog.ILogger _log = Log.ForContext<CustomerCreditsController>();

    public CustomerCreditsController(ICreditService creditService)
    {
        _creditService = creditService;
    }

    /// <summary>
    /// Retrieves the credit balance for a customer
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>The customer credit details</returns>
    /// <response code="200">Returns the customer credit</response>
    /// <response code="404">If the customer credit is not found</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("customer/{customerId}")]
    [Authorize(Policy = "CustomerCredit.Read")]
    [ProducesResponseType(typeof(CustomerCreditDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerCreditDto>> GetCustomerCredit(int customerId)
    {
        try
        {
            _log.Information("API: GetCustomerCredit called for customer: {CustomerId} by user {User}", 
                customerId, User.Identity?.Name);
            var credit = await _creditService.GetCustomerCreditAsync(customerId);

            if (credit == null)
            {
                _log.Warning("API: Customer credit not found for customer {CustomerId}", customerId);
                return NotFound($"Customer credit not found for customer {customerId}");
            }

            return Ok(credit);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCustomerCredit for customer: {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving customer credit");
        }
    }

    /// <summary>
    /// Retrieves all credit transactions for a customer
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <returns>List of credit transactions</returns>
    /// <response code="200">Returns the list of credit transactions</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("customer/{customerId}/transactions")]
    [Authorize(Policy = "CustomerCredit.Read")]
    [ProducesResponseType(typeof(IEnumerable<CreditTransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<CreditTransactionDto>>> GetCreditTransactions(int customerId)
    {
        try
        {
            _log.Information("API: GetCreditTransactions called for customer: {CustomerId} by user {User}", 
                customerId, User.Identity?.Name);
            var transactions = await _creditService.GetCreditTransactionsAsync(customerId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetCreditTransactions for customer: {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving credit transactions");
        }
    }

    /// <summary>
    /// Creates a new credit transaction
    /// </summary>
    /// <param name="createDto">The credit transaction creation data</param>
    /// <returns>The created credit transaction</returns>
    /// <response code="201">Returns the newly created credit transaction</response>
    /// <response code="400">If the credit transaction data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("transactions")]
    [Authorize(Policy = "CustomerCredit.Write")]
    [ProducesResponseType(typeof(CreditTransactionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreditTransactionDto>> CreateCreditTransaction([FromBody] CreateCreditTransactionDto createDto)
    {
        try
        {
            _log.Information("API: CreateCreditTransaction called for customer: {CustomerId} by user {User}", 
                createDto.CustomerId, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get user ID from claims
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                _log.Warning("API: Unable to extract user ID from claims");
                return BadRequest("Unable to identify user");
            }

            var transaction = await _creditService.CreateCreditTransactionAsync(createDto, userId);
            
            _log.Information("API: Credit transaction created with ID: {TransactionId}", transaction.Id);
            return CreatedAtAction(nameof(GetCreditTransactions), 
                new { customerId = createDto.CustomerId }, transaction);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateCreditTransaction");
            return StatusCode(500, "An error occurred while creating the credit transaction");
        }
    }

    /// <summary>
    /// Adds credit to a customer account
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="amount">The amount to add</param>
    /// <param name="description">The description</param>
    /// <returns>The new credit balance</returns>
    /// <response code="200">Returns the new credit balance</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("customer/{customerId}/add")]
    [Authorize(Policy = "CustomerCredit.Write")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<decimal>> AddCredit(int customerId, [FromQuery] decimal amount, [FromQuery] string description)
    {
        try
        {
            _log.Information("API: AddCredit called for customer: {CustomerId}, amount: {Amount} by user {User}", 
                customerId, amount, User.Identity?.Name);

            if (amount <= 0)
            {
                return BadRequest("Amount must be greater than zero");
            }

            // Get user ID from claims
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                _log.Warning("API: Unable to extract user ID from claims");
                return BadRequest("Unable to identify user");
            }

            var newBalance = await _creditService.AddCreditAsync(customerId, amount, description, userId);
            
            _log.Information("API: Credit added to customer {CustomerId}, new balance: {Balance}", customerId, newBalance);
            return Ok(new { customerId, amount, newBalance, description });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in AddCredit for customer: {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while adding credit");
        }
    }

    /// <summary>
    /// Deducts credit from a customer account
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="amount">The amount to deduct</param>
    /// <param name="invoiceId">The invoice ID (optional)</param>
    /// <param name="description">The description</param>
    /// <returns>The new credit balance</returns>
    /// <response code="200">Returns the new credit balance</response>
    /// <response code="400">If the request is invalid or insufficient credit</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required role</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("customer/{customerId}/deduct")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<decimal>> DeductCredit(int customerId, [FromQuery] decimal amount, 
        [FromQuery] int? invoiceId, [FromQuery] string description)
    {
        try
        {
            _log.Information("API: DeductCredit called for customer: {CustomerId}, amount: {Amount} by user {User}", 
                customerId, amount, User.Identity?.Name);

            if (amount <= 0)
            {
                return BadRequest("Amount must be greater than zero");
            }

            // Get user ID from claims
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                _log.Warning("API: Unable to extract user ID from claims");
                return BadRequest("Unable to identify user");
            }

            var newBalance = await _creditService.DeductCreditAsync(customerId, amount, invoiceId, description, userId);
            
            _log.Information("API: Credit deducted from customer {CustomerId}, new balance: {Balance}", 
                customerId, newBalance);
            return Ok(new { customerId, amount, newBalance, description });
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "API: Insufficient credit for customer: {CustomerId}", customerId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeductCredit for customer: {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while deducting credit");
        }
    }

    /// <summary>
    /// Checks if a customer has sufficient credit
    /// </summary>
    /// <param name="customerId">The customer ID</param>
    /// <param name="amount">The required amount</param>
    /// <returns>True if sufficient credit, otherwise false</returns>
    /// <response code="200">Returns the result</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("customer/{customerId}/check")]
    [Authorize]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> HasSufficientCredit(int customerId, [FromQuery] decimal amount)
    {
        try
        {
            _log.Information("API: HasSufficientCredit called for customer: {CustomerId}, amount: {Amount} by user {User}", 
                customerId, amount, User.Identity?.Name);

            var hasSufficientCredit = await _creditService.HasSufficientCreditAsync(customerId, amount);
            
            return Ok(new { customerId, amount, hasSufficientCredit });
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in HasSufficientCredit for customer: {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while checking credit balance");
        }
    }
}
