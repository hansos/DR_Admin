using ISPAdmin.DTOs;

namespace ISPAdmin.Services;

/// <summary>
/// Service interface for managing tax categories.
/// </summary>
public interface ITaxCategoryService
{
    /// <summary>
    /// Retrieves all tax categories.
    /// </summary>
    /// <returns>Collection of tax category DTOs.</returns>
    Task<IEnumerable<TaxCategoryDto>> GetAllTaxCategoriesAsync();

    /// <summary>
    /// Retrieves a tax category by identifier.
    /// </summary>
    /// <param name="id">Tax category identifier.</param>
    /// <returns>Tax category DTO if found; otherwise null.</returns>
    Task<TaxCategoryDto?> GetTaxCategoryByIdAsync(int id);

    /// <summary>
    /// Creates a tax category.
    /// </summary>
    /// <param name="dto">Create tax category payload.</param>
    /// <returns>Created tax category DTO.</returns>
    Task<TaxCategoryDto> CreateTaxCategoryAsync(CreateTaxCategoryDto dto);

    /// <summary>
    /// Updates a tax category.
    /// </summary>
    /// <param name="id">Tax category identifier.</param>
    /// <param name="dto">Update tax category payload.</param>
    /// <returns>Updated tax category DTO if found; otherwise null.</returns>
    Task<TaxCategoryDto?> UpdateTaxCategoryAsync(int id, UpdateTaxCategoryDto dto);

    /// <summary>
    /// Deletes a tax category.
    /// </summary>
    /// <param name="id">Tax category identifier.</param>
    /// <returns>True when deleted; otherwise false.</returns>
    Task<bool> DeleteTaxCategoryAsync(int id);
}
