using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages vendor tax profiles
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class VendorTaxProfilesController : ControllerBase
{
    private readonly IVendorTaxProfileService _vendorTaxProfileService;
    private static readonly Serilog.ILogger _log = Log.ForContext<VendorTaxProfilesController>();

    public VendorTaxProfilesController(IVendorTaxProfileService vendorTaxProfileService)
    {
        _vendorTaxProfileService = vendorTaxProfileService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(IEnumerable<VendorTaxProfileDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VendorTaxProfileDto>>> GetAllVendorTaxProfiles()
    {
        try
        {
            _log.Information("API: GetAllVendorTaxProfiles called by user {User}", User.Identity?.Name);
            var profiles = await _vendorTaxProfileService.GetAllVendorTaxProfilesAsync();
            return Ok(profiles);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetAllVendorTaxProfiles");
            return StatusCode(500, "An error occurred while retrieving vendor tax profiles");
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Finance,Support")]
    [ProducesResponseType(typeof(VendorTaxProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorTaxProfileDto>> GetVendorTaxProfileById(int id)
    {
        try
        {
            _log.Information("API: GetVendorTaxProfileById called for ID: {Id}", id);
            var profile = await _vendorTaxProfileService.GetVendorTaxProfileByIdAsync(id);
            if (profile == null)
                return NotFound($"Vendor tax profile with ID {id} not found");
            return Ok(profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetVendorTaxProfileById for ID: {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the vendor tax profile");
        }
    }

    [HttpGet("vendor/{vendorId}")]
    [Authorize(Roles = "Admin,Finance,Support")]
    [ProducesResponseType(typeof(VendorTaxProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorTaxProfileDto>> GetVendorTaxProfileByVendorId(int vendorId)
    {
        try
        {
            _log.Information("API: GetVendorTaxProfileByVendorId called for vendor: {VendorId}", vendorId);
            var profile = await _vendorTaxProfileService.GetVendorTaxProfileByVendorIdAsync(vendorId);
            if (profile == null)
                return NotFound($"Vendor tax profile for vendor ID {vendorId} not found");
            return Ok(profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in GetVendorTaxProfileByVendorId");
            return StatusCode(500, "An error occurred while retrieving the vendor tax profile");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(VendorTaxProfileDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VendorTaxProfileDto>> CreateVendorTaxProfile([FromBody] CreateVendorTaxProfileDto createDto)
    {
        try
        {
            _log.Information("API: CreateVendorTaxProfile called for vendor: {VendorId}", createDto.VendorId);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var profile = await _vendorTaxProfileService.CreateVendorTaxProfileAsync(createDto);
            _log.Information("API: Vendor tax profile created with ID: {Id}", profile.Id);
            return CreatedAtAction(nameof(GetVendorTaxProfileById), new { id = profile.Id }, profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in CreateVendorTaxProfile");
            return StatusCode(500, "An error occurred while creating the vendor tax profile");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Finance")]
    [ProducesResponseType(typeof(VendorTaxProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorTaxProfileDto>> UpdateVendorTaxProfile(int id, [FromBody] UpdateVendorTaxProfileDto updateDto)
    {
        try
        {
            _log.Information("API: UpdateVendorTaxProfile called for ID: {Id}", id);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var profile = await _vendorTaxProfileService.UpdateVendorTaxProfileAsync(id, updateDto);
            if (profile == null)
                return NotFound($"Vendor tax profile with ID {id} not found");

            return Ok(profile);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in UpdateVendorTaxProfile for ID: {Id}", id);
            return StatusCode(500, "An error occurred while updating the vendor tax profile");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVendorTaxProfile(int id)
    {
        try
        {
            _log.Information("API: DeleteVendorTaxProfile called for ID: {Id}", id);
            var result = await _vendorTaxProfileService.DeleteVendorTaxProfileAsync(id);
            if (!result)
                return NotFound($"Vendor tax profile with ID {id} not found");
            return NoContent();
        }
        catch (Exception ex)
        {
            _log.Error(ex, "API: Error in DeleteVendorTaxProfile for ID: {Id}", id);
            return StatusCode(500, "An error occurred while deleting the vendor tax profile");
        }
    }
}
