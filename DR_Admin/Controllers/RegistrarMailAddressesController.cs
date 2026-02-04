using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages registrar mail address information including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("api/v1/customers/{customerId}/registrar-mail-addresses")]
[Authorize]
public class RegistrarMailAddressesController : ControllerBase
{
    private readonly IRegistrarMailAddressService _registrarMailAddressService;
    private static readonly Serilog.ILogger _log = Log.ForContext<RegistrarMailAddressesController>();

    public RegistrarMailAddressesController(IRegistrarMailAddressService registrarMailAddressService)
    {
        _registrarMailAddressService = registrarMailAddressService;
    }

    /// <summary>
    /// Retrieves all registrar mail addresses for a specific customer
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <returns>List of all registrar mail addresses for the customer</returns>
    /// <response code="200">Returns the list of registrar mail addresses</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet]
    [Authorize(Policy = "RegistrarMailAddress.Read")]
    [ProducesResponseType(typeof(IEnumerable<RegistrarMailAddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetRegistrarMailAddresses(int customerId)
    {
        try
        {
            _log.Information("API: GetRegistrarMailAddresses called for customer ID {CustomerId} by user {User}", customerId, User.Identity?.Name);
            
            var mailAddresses = await _registrarMailAddressService.GetRegistrarMailAddressesAsync(customerId);
            return Ok(mailAddresses);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegistrarMailAddresses for customer ID {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving registrar mail addresses");
        }
    }

    /// <summary>
    /// Retrieves the default mail address for a specific customer
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <returns>The default registrar mail address</returns>
    /// <response code="200">Returns the default registrar mail address</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="404">If default mail address is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("default")]
    [Authorize(Policy = "RegistrarMailAddress.Read")]
    [ProducesResponseType(typeof(RegistrarMailAddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarMailAddressDto>> GetDefaultMailAddress(int customerId)
    {
        try
        {
            _log.Information("API: GetDefaultMailAddress called for customer ID {CustomerId} by user {User}", customerId, User.Identity?.Name);
            
            var mailAddress = await _registrarMailAddressService.GetDefaultMailAddressAsync(customerId);

            if (mailAddress == null)
            {
                _log.Information("API: Default mail address for customer ID {CustomerId} not found", customerId);
                return NotFound($"Default mail address for customer ID {customerId} not found");
            }

            return Ok(mailAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetDefaultMailAddress for customer ID {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while retrieving the default mail address");
        }
    }

    /// <summary>
    /// Retrieves a specific registrar mail address by its unique identifier
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <param name="id">The unique identifier of the registrar mail address</param>
    /// <returns>The registrar mail address information</returns>
    /// <response code="200">Returns the registrar mail address data</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="404">If registrar mail address is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("{id}")]
    [Authorize(Policy = "RegistrarMailAddress.Read")]
    [ProducesResponseType(typeof(RegistrarMailAddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarMailAddressDto>> GetRegistrarMailAddressById(int customerId, int id)
    {
        try
        {
            _log.Information("API: GetRegistrarMailAddressById called for ID {MailAddressId} and customer ID {CustomerId} by user {User}", 
                id, customerId, User.Identity?.Name);
            
            var mailAddress = await _registrarMailAddressService.GetRegistrarMailAddressByIdAsync(id);

            if (mailAddress == null || mailAddress.CustomerId != customerId)
            {
                _log.Information("API: Registrar mail address with ID {MailAddressId} not found for customer ID {CustomerId}", id, customerId);
                return NotFound($"Registrar mail address with ID {id} not found");
            }

            return Ok(mailAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetRegistrarMailAddressById for ID {MailAddressId}", id);
            return StatusCode(500, "An error occurred while retrieving the registrar mail address");
        }
    }

    /// <summary>
    /// Creates a new registrar mail address
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <param name="createDto">Registrar mail address information for creation</param>
    /// <returns>The newly created registrar mail address</returns>
    /// <response code="201">Returns the newly created registrar mail address</response>
    /// <response code="400">If the registrar mail address data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost]
    [Authorize(Policy = "RegistrarMailAddress.Write")]
    [ProducesResponseType(typeof(RegistrarMailAddressDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarMailAddressDto>> CreateRegistrarMailAddress(int customerId, [FromBody] CreateRegistrarMailAddressDto createDto)
    {
        try
        {
            _log.Information("API: CreateRegistrarMailAddress called for customer ID {CustomerId} by user {User}", customerId, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for CreateRegistrarMailAddress");
                return BadRequest(ModelState);
            }

            var mailAddress = await _registrarMailAddressService.CreateRegistrarMailAddressAsync(customerId, createDto);
            
            return CreatedAtAction(
                nameof(GetRegistrarMailAddressById),
                new { customerId = customerId, id = mailAddress.Id },
                mailAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateRegistrarMailAddress for customer ID {CustomerId}", customerId);
            return StatusCode(500, "An error occurred while creating the registrar mail address");
        }
    }

    /// <summary>
    /// Updates an existing registrar mail address
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <param name="id">The unique identifier of the registrar mail address to update</param>
    /// <param name="updateDto">Updated registrar mail address information</param>
    /// <returns>The updated registrar mail address</returns>
    /// <response code="200">Returns the updated registrar mail address</response>
    /// <response code="400">If the registrar mail address data is invalid</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="404">If registrar mail address is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}")]
    [Authorize(Policy = "RegistrarMailAddress.Write")]
    [ProducesResponseType(typeof(RegistrarMailAddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarMailAddressDto>> UpdateRegistrarMailAddress(int customerId, int id, [FromBody] UpdateRegistrarMailAddressDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateRegistrarMailAddress called for ID {MailAddressId} and customer ID {CustomerId} by user {User}", 
                id, customerId, User.Identity?.Name);

            if (!ModelState.IsValid)
            {
                _log.Warning("API: Invalid model state for UpdateRegistrarMailAddress");
                return BadRequest(ModelState);
            }

            var mailAddress = await _registrarMailAddressService.UpdateRegistrarMailAddressAsync(id, updateDto);

            if (mailAddress == null || mailAddress.CustomerId != customerId)
            {
                _log.Information("API: Registrar mail address with ID {MailAddressId} not found for customer ID {CustomerId}", id, customerId);
                return NotFound($"Registrar mail address with ID {id} not found");
            }

            return Ok(mailAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateRegistrarMailAddress for ID {MailAddressId}", id);
            return StatusCode(500, "An error occurred while updating the registrar mail address");
        }
    }

    /// <summary>
    /// Deletes a registrar mail address
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <param name="id">The unique identifier of the registrar mail address to delete</param>
    /// <returns>No content if successful</returns>
    /// <response code="204">If the registrar mail address was successfully deleted</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="404">If registrar mail address is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpDelete("{id}")]
    [Authorize(Policy = "RegistrarMailAddress.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteRegistrarMailAddress(int customerId, int id)
    {
        try
        {
            _log.Information("API: DeleteRegistrarMailAddress called for ID {MailAddressId} and customer ID {CustomerId} by user {User}", 
                id, customerId, User.Identity?.Name);

            var mailAddress = await _registrarMailAddressService.GetRegistrarMailAddressByIdAsync(id);

            if (mailAddress == null || mailAddress.CustomerId != customerId)
            {
                _log.Information("API: Registrar mail address with ID {MailAddressId} not found for customer ID {CustomerId}", id, customerId);
                return NotFound($"Registrar mail address with ID {id} not found");
            }

            var deleted = await _registrarMailAddressService.DeleteRegistrarMailAddressAsync(id);

            if (!deleted)
            {
                _log.Warning("API: Failed to delete registrar mail address with ID {MailAddressId}", id);
                return NotFound($"Registrar mail address with ID {id} not found");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteRegistrarMailAddress for ID {MailAddressId}", id);
            return StatusCode(500, "An error occurred while deleting the registrar mail address");
        }
    }

    /// <summary>
    /// Sets a registrar mail address as the default for the customer
    /// </summary>
    /// <param name="customerId">The unique identifier of the customer</param>
    /// <param name="id">The unique identifier of the registrar mail address</param>
    /// <returns>The updated registrar mail address</returns>
    /// <response code="200">Returns the updated registrar mail address</response>
    /// <response code="401">If user is not authenticated</response>
    /// <response code="403">If user doesn't have required permissions</response>
    /// <response code="404">If registrar mail address is not found</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPut("{id}/set-default")]
    [Authorize(Policy = "RegistrarMailAddress.Write")]
    [ProducesResponseType(typeof(RegistrarMailAddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RegistrarMailAddressDto>> SetDefaultMailAddress(int customerId, int id)
    {
        try
        {
            _log.Information("API: SetDefaultMailAddress called for ID {MailAddressId} and customer ID {CustomerId} by user {User}", 
                id, customerId, User.Identity?.Name);

            var mailAddress = await _registrarMailAddressService.SetDefaultMailAddressAsync(id);

            if (mailAddress == null || mailAddress.CustomerId != customerId)
            {
                _log.Information("API: Registrar mail address with ID {MailAddressId} not found for customer ID {CustomerId}", id, customerId);
                return NotFound($"Registrar mail address with ID {id} not found");
            }

            return Ok(mailAddress);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in SetDefaultMailAddress for ID {MailAddressId}", id);
            return StatusCode(500, "An error occurred while setting the default mail address");
        }
    }
}
