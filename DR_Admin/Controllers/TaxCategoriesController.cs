using ISPAdmin.DTOs;
using ISPAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ISPAdmin.Controllers;

/// <summary>
/// Manages tax categories by country and optional state.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TaxCategoriesController : ControllerBase
{
    private readonly ITaxCategoryService _taxCategoryService;

    public TaxCategoriesController(ITaxCategoryService taxCategoryService)
    {
        _taxCategoryService = taxCategoryService;
    }

    /// <summary>
    /// Retrieves all tax categories.
    /// </summary>
    /// <returns>List of tax categories.</returns>
    [HttpGet]
    [Authorize(Policy = "TaxCategory.Read")]
    [ProducesResponseType(typeof(IEnumerable<TaxCategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TaxCategoryDto>>> GetAllTaxCategories()
    {
        var result = await _taxCategoryService.GetAllTaxCategoriesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a tax category by identifier.
    /// </summary>
    /// <param name="id">Tax category identifier.</param>
    /// <returns>Tax category details.</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "TaxCategory.Read")]
    [ProducesResponseType(typeof(TaxCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaxCategoryDto>> GetTaxCategoryById(int id)
    {
        var result = await _taxCategoryService.GetTaxCategoryByIdAsync(id);
        if (result == null)
        {
            return NotFound($"Tax category with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Creates a tax category.
    /// </summary>
    /// <param name="dto">Create tax category payload.</param>
    /// <returns>Created tax category.</returns>
    [HttpPost]
    [Authorize(Policy = "TaxCategory.Write")]
    [ProducesResponseType(typeof(TaxCategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaxCategoryDto>> CreateTaxCategory([FromBody] CreateTaxCategoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _taxCategoryService.CreateTaxCategoryAsync(dto);
        return CreatedAtAction(nameof(GetTaxCategoryById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates a tax category.
    /// </summary>
    /// <param name="id">Tax category identifier.</param>
    /// <param name="dto">Update tax category payload.</param>
    /// <returns>Updated tax category.</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "TaxCategory.Write")]
    [ProducesResponseType(typeof(TaxCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaxCategoryDto>> UpdateTaxCategory(int id, [FromBody] UpdateTaxCategoryDto dto)
    {
        var result = await _taxCategoryService.UpdateTaxCategoryAsync(id, dto);
        if (result == null)
        {
            return NotFound($"Tax category with ID {id} not found");
        }

        return Ok(result);
    }

    /// <summary>
    /// Deletes a tax category.
    /// </summary>
    /// <param name="id">Tax category identifier.</param>
    /// <returns>No content when successful.</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "TaxCategory.Delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTaxCategory(int id)
    {
        var deleted = await _taxCategoryService.DeleteTaxCategoryAsync(id);
        if (!deleted)
        {
            return NotFound($"Tax category with ID {id} not found");
        }

        return NoContent();
    }
}
