using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ISPAdmin.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class PaymentInstrumentsController : ControllerBase
{
    private readonly IPaymentInstrumentService _service;
    private static readonly Serilog.ILogger _log = Log.ForContext<PaymentInstrumentsController>();

    public PaymentInstrumentsController(IPaymentInstrumentService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = "PaymentGateway.Read")]
    [ProducesResponseType(typeof(IEnumerable<PaymentInstrumentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentInstrumentDto>>> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<PaymentInstrumentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentInstrumentDto>>> GetActive()
    {
        var items = await _service.GetActiveAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "PaymentGateway.Read")]
    [ProducesResponseType(typeof(PaymentInstrumentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentInstrumentDto>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PaymentInstrumentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentInstrumentDto>> Create([FromBody] CreatePaymentInstrumentDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "Create payment instrument validation error");
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PaymentInstrumentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentInstrumentDto>> Update(int id, [FromBody] UpdatePaymentInstrumentDto dto)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "Update payment instrument validation error");
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _log.Warning(ex, "Delete payment instrument validation error");
            return BadRequest(ex.Message);
        }
    }
}
