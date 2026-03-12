using ISPAdmin.Data.Enums;

namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object for profit margin settings.
/// </summary>
public class ProfitMarginSettingDto
{
    /// <summary>
    /// Unique identifier of the margin setting.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Product class this margin applies to.
    /// </summary>
    public ProfitProductClass ProductClass { get; set; }

    /// <summary>
    /// Profit percentage for this product class.
    /// </summary>
    public decimal ProfitPercent { get; set; }

    /// <summary>
    /// Whether this margin setting is active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Optional notes for this margin setting.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// When this record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When this record was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a profit margin setting.
/// </summary>
public class CreateProfitMarginSettingDto
{
    /// <summary>
    /// Product class this margin applies to.
    /// </summary>
    public ProfitProductClass ProductClass { get; set; }

    /// <summary>
    /// Profit percentage for this product class.
    /// </summary>
    public decimal ProfitPercent { get; set; }

    /// <summary>
    /// Whether this margin setting is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional notes for this margin setting.
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Data transfer object for updating a profit margin setting.
/// </summary>
public class UpdateProfitMarginSettingDto
{
    /// <summary>
    /// Profit percentage for this product class.
    /// </summary>
    public decimal ProfitPercent { get; set; }

    /// <summary>
    /// Whether this margin setting is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional notes for this margin setting.
    /// </summary>
    public string? Notes { get; set; }
}
