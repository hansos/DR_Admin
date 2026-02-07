namespace ISPAdmin.DTOs;

// ==================== RegistrarTldCostPricing DTOs ====================

/// <summary>
/// DTO for displaying registrar TLD cost pricing information
/// </summary>
public class RegistrarTldCostPricingDto
{
    /// <summary>
    /// Unique identifier for the cost pricing record
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to the RegistrarTld relationship
    /// </summary>
    public int RegistrarTldId { get; set; }

    /// <summary>
    /// Name of the registrar
    /// </summary>
    public string? RegistrarName { get; set; }

    /// <summary>
    /// TLD extension (e.g., "com", "net")
    /// </summary>
    public string? TldExtension { get; set; }

    /// <summary>
    /// The date when this pricing becomes effective (UTC)
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// The date when this pricing expires (UTC), null if current/future
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Cost for domain registration
    /// </summary>
    public decimal RegistrationCost { get; set; }

    /// <summary>
    /// Cost for domain renewal
    /// </summary>
    public decimal RenewalCost { get; set; }

    /// <summary>
    /// Cost for domain transfer
    /// </summary>
    public decimal TransferCost { get; set; }

    /// <summary>
    /// Cost for privacy/WHOIS protection
    /// </summary>
    public decimal? PrivacyCost { get; set; }

    /// <summary>
    /// Special cost for first-year registration
    /// </summary>
    public decimal? FirstYearRegistrationCost { get; set; }

    /// <summary>
    /// Currency code (ISO 4217)
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Whether this pricing is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// User who created this record
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// When this record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When this record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating new registrar TLD cost pricing
/// </summary>
public class CreateRegistrarTldCostPricingDto
{
    /// <summary>
    /// Foreign key to the RegistrarTld relationship
    /// </summary>
    public int RegistrarTldId { get; set; }

    /// <summary>
    /// The date when this pricing becomes effective (UTC)
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// The date when this pricing expires (UTC), null if no expiry
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Cost for domain registration
    /// </summary>
    public decimal RegistrationCost { get; set; }

    /// <summary>
    /// Cost for domain renewal
    /// </summary>
    public decimal RenewalCost { get; set; }

    /// <summary>
    /// Cost for domain transfer
    /// </summary>
    public decimal TransferCost { get; set; }

    /// <summary>
    /// Cost for privacy/WHOIS protection
    /// </summary>
    public decimal? PrivacyCost { get; set; }

    /// <summary>
    /// Special cost for first-year registration
    /// </summary>
    public decimal? FirstYearRegistrationCost { get; set; }

    /// <summary>
    /// Currency code (ISO 4217)
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Whether this pricing is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for updating existing registrar TLD cost pricing
/// </summary>
public class UpdateRegistrarTldCostPricingDto
{
    /// <summary>
    /// The date when this pricing becomes effective (UTC)
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// The date when this pricing expires (UTC), null if no expiry
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Cost for domain registration
    /// </summary>
    public decimal RegistrationCost { get; set; }

    /// <summary>
    /// Cost for domain renewal
    /// </summary>
    public decimal RenewalCost { get; set; }

    /// <summary>
    /// Cost for domain transfer
    /// </summary>
    public decimal TransferCost { get; set; }

    /// <summary>
    /// Cost for privacy/WHOIS protection
    /// </summary>
    public decimal? PrivacyCost { get; set; }

    /// <summary>
    /// Special cost for first-year registration
    /// </summary>
    public decimal? FirstYearRegistrationCost { get; set; }

    /// <summary>
    /// Currency code (ISO 4217)
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Whether this pricing is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; set; }
}

// ==================== TldSalesPricing DTOs ====================

/// <summary>
/// DTO for displaying TLD sales pricing information
/// </summary>
public class TldSalesPricingDto
{
    /// <summary>
    /// Unique identifier for the sales pricing record
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to the TLD
    /// </summary>
    public int TldId { get; set; }

    /// <summary>
    /// TLD extension (e.g., "com", "net")
    /// </summary>
    public string? TldExtension { get; set; }

    /// <summary>
    /// The date when this pricing becomes effective (UTC)
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// The date when this pricing expires (UTC), null if current/future
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Price for domain registration
    /// </summary>
    public decimal RegistrationPrice { get; set; }

    /// <summary>
    /// Price for domain renewal
    /// </summary>
    public decimal RenewalPrice { get; set; }

    /// <summary>
    /// Price for domain transfer
    /// </summary>
    public decimal TransferPrice { get; set; }

    /// <summary>
    /// Price for privacy/WHOIS protection
    /// </summary>
    public decimal? PrivacyPrice { get; set; }

    /// <summary>
    /// Special price for first-year registration
    /// </summary>
    public decimal? FirstYearRegistrationPrice { get; set; }

    /// <summary>
    /// Currency code (ISO 4217)
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Whether this is promotional pricing
    /// </summary>
    public bool IsPromotional { get; set; }

    /// <summary>
    /// Name of the promotion
    /// </summary>
    public string? PromotionName { get; set; }

    /// <summary>
    /// Whether this pricing is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// User who created this record
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// When this record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When this record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating new TLD sales pricing
/// </summary>
public class CreateTldSalesPricingDto
{
    /// <summary>
    /// Foreign key to the TLD
    /// </summary>
    public int TldId { get; set; }

    /// <summary>
    /// The date when this pricing becomes effective (UTC)
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// The date when this pricing expires (UTC), null if no expiry
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Price for domain registration
    /// </summary>
    public decimal RegistrationPrice { get; set; }

    /// <summary>
    /// Price for domain renewal
    /// </summary>
    public decimal RenewalPrice { get; set; }

    /// <summary>
    /// Price for domain transfer
    /// </summary>
    public decimal TransferPrice { get; set; }

    /// <summary>
    /// Price for privacy/WHOIS protection
    /// </summary>
    public decimal? PrivacyPrice { get; set; }

    /// <summary>
    /// Special price for first-year registration
    /// </summary>
    public decimal? FirstYearRegistrationPrice { get; set; }

    /// <summary>
    /// Currency code (ISO 4217)
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Whether this is promotional pricing
    /// </summary>
    public bool IsPromotional { get; set; } = false;

    /// <summary>
    /// Name of the promotion
    /// </summary>
    public string? PromotionName { get; set; }

    /// <summary>
    /// Whether this pricing is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for updating existing TLD sales pricing
/// </summary>
public class UpdateTldSalesPricingDto
{
    /// <summary>
    /// The date when this pricing becomes effective (UTC)
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// The date when this pricing expires (UTC), null if no expiry
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Price for domain registration
    /// </summary>
    public decimal RegistrationPrice { get; set; }

    /// <summary>
    /// Price for domain renewal
    /// </summary>
    public decimal RenewalPrice { get; set; }

    /// <summary>
    /// Price for domain transfer
    /// </summary>
    public decimal TransferPrice { get; set; }

    /// <summary>
    /// Price for privacy/WHOIS protection
    /// </summary>
    public decimal? PrivacyPrice { get; set; }

    /// <summary>
    /// Special price for first-year registration
    /// </summary>
    public decimal? FirstYearRegistrationPrice { get; set; }

    /// <summary>
    /// Currency code (ISO 4217)
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Whether this is promotional pricing
    /// </summary>
    public bool IsPromotional { get; set; }

    /// <summary>
    /// Name of the promotion
    /// </summary>
    public string? PromotionName { get; set; }

    /// <summary>
    /// Whether this pricing is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; set; }
}

// ==================== ResellerTldDiscount DTOs ====================

/// <summary>
/// DTO for displaying reseller TLD discount information
/// </summary>
public class ResellerTldDiscountDto
{
    /// <summary>
    /// Unique identifier for the discount record
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to the ResellerCompany
    /// </summary>
    public int ResellerCompanyId { get; set; }

    /// <summary>
    /// Name of the reseller company
    /// </summary>
    public string? ResellerCompanyName { get; set; }

    /// <summary>
    /// Foreign key to the TLD
    /// </summary>
    public int TldId { get; set; }

    /// <summary>
    /// TLD extension (e.g., "com", "net")
    /// </summary>
    public string? TldExtension { get; set; }

    /// <summary>
    /// The date when this discount becomes effective (UTC)
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// The date when this discount expires (UTC), null if no expiry
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Discount percentage (e.g., 10.5 for 10.5% off)
    /// </summary>
    public decimal? DiscountPercentage { get; set; }

    /// <summary>
    /// Fixed discount amount (e.g., 2.00 for $2.00 off)
    /// </summary>
    public decimal? DiscountAmount { get; set; }

    /// <summary>
    /// Currency code for DiscountAmount
    /// </summary>
    public string? DiscountCurrency { get; set; }

    /// <summary>
    /// Whether discount applies to registration
    /// </summary>
    public bool ApplyToRegistration { get; set; }

    /// <summary>
    /// Whether discount applies to renewal
    /// </summary>
    public bool ApplyToRenewal { get; set; }

    /// <summary>
    /// Whether discount applies to transfer
    /// </summary>
    public bool ApplyToTransfer { get; set; }

    /// <summary>
    /// Whether this discount is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// User who created this record
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// When this record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When this record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating new reseller TLD discount
/// </summary>
public class CreateResellerTldDiscountDto
{
    /// <summary>
    /// Foreign key to the ResellerCompany
    /// </summary>
    public int ResellerCompanyId { get; set; }

    /// <summary>
    /// Foreign key to the TLD
    /// </summary>
    public int TldId { get; set; }

    /// <summary>
    /// The date when this discount becomes effective (UTC)
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// The date when this discount expires (UTC), null if no expiry
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Discount percentage (e.g., 10.5 for 10.5% off)
    /// </summary>
    public decimal? DiscountPercentage { get; set; }

    /// <summary>
    /// Fixed discount amount (e.g., 2.00 for $2.00 off)
    /// </summary>
    public decimal? DiscountAmount { get; set; }

    /// <summary>
    /// Currency code for DiscountAmount
    /// </summary>
    public string? DiscountCurrency { get; set; }

    /// <summary>
    /// Whether discount applies to registration
    /// </summary>
    public bool ApplyToRegistration { get; set; } = true;

    /// <summary>
    /// Whether discount applies to renewal
    /// </summary>
    public bool ApplyToRenewal { get; set; } = true;

    /// <summary>
    /// Whether discount applies to transfer
    /// </summary>
    public bool ApplyToTransfer { get; set; } = false;

    /// <summary>
    /// Whether this discount is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for updating existing reseller TLD discount
/// </summary>
public class UpdateResellerTldDiscountDto
{
    /// <summary>
    /// The date when this discount becomes effective (UTC)
    /// </summary>
    public DateTime EffectiveFrom { get; set; }

    /// <summary>
    /// The date when this discount expires (UTC), null if no expiry
    /// </summary>
    public DateTime? EffectiveTo { get; set; }

    /// <summary>
    /// Discount percentage (e.g., 10.5 for 10.5% off)
    /// </summary>
    public decimal? DiscountPercentage { get; set; }

    /// <summary>
    /// Fixed discount amount (e.g., 2.00 for $2.00 off)
    /// </summary>
    public decimal? DiscountAmount { get; set; }

    /// <summary>
    /// Currency code for DiscountAmount
    /// </summary>
    public string? DiscountCurrency { get; set; }

    /// <summary>
    /// Whether discount applies to registration
    /// </summary>
    public bool ApplyToRegistration { get; set; }

    /// <summary>
    /// Whether discount applies to renewal
    /// </summary>
    public bool ApplyToRenewal { get; set; }

    /// <summary>
    /// Whether discount applies to transfer
    /// </summary>
    public bool ApplyToTransfer { get; set; }

    /// <summary>
    /// Whether this discount is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; set; }
}

// ==================== RegistrarSelectionPreference DTOs ====================

/// <summary>
/// DTO for displaying registrar selection preference information
/// </summary>
public class RegistrarSelectionPreferenceDto
{
    /// <summary>
    /// Unique identifier for the preference record
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key to the Registrar
    /// </summary>
    public int RegistrarId { get; set; }

    /// <summary>
    /// Name of the registrar
    /// </summary>
    public string? RegistrarName { get; set; }

    /// <summary>
    /// Priority level (lower = higher priority)
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Whether registrar offers hosting services
    /// </summary>
    public bool OffersHosting { get; set; }

    /// <summary>
    /// Whether registrar offers email services
    /// </summary>
    public bool OffersEmail { get; set; }

    /// <summary>
    /// Whether registrar offers SSL certificates
    /// </summary>
    public bool OffersSsl { get; set; }

    /// <summary>
    /// Maximum cost difference for bundling preference
    /// </summary>
    public decimal? MaxCostDifferenceThreshold { get; set; }

    /// <summary>
    /// Prefer for hosting customers
    /// </summary>
    public bool PreferForHostingCustomers { get; set; }

    /// <summary>
    /// Prefer for email customers
    /// </summary>
    public bool PreferForEmailCustomers { get; set; }

    /// <summary>
    /// Whether this preference is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// When this record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When this record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating new registrar selection preference
/// </summary>
public class CreateRegistrarSelectionPreferenceDto
{
    /// <summary>
    /// Foreign key to the Registrar
    /// </summary>
    public int RegistrarId { get; set; }

    /// <summary>
    /// Priority level (lower = higher priority)
    /// </summary>
    public int Priority { get; set; } = 100;

    /// <summary>
    /// Whether registrar offers hosting services
    /// </summary>
    public bool OffersHosting { get; set; } = false;

    /// <summary>
    /// Whether registrar offers email services
    /// </summary>
    public bool OffersEmail { get; set; } = false;

    /// <summary>
    /// Whether registrar offers SSL certificates
    /// </summary>
    public bool OffersSsl { get; set; } = false;

    /// <summary>
    /// Maximum cost difference for bundling preference
    /// </summary>
    public decimal? MaxCostDifferenceThreshold { get; set; } = 2.00m;

    /// <summary>
    /// Prefer for hosting customers
    /// </summary>
    public bool PreferForHostingCustomers { get; set; } = false;

    /// <summary>
    /// Prefer for email customers
    /// </summary>
    public bool PreferForEmailCustomers { get; set; } = false;

    /// <summary>
    /// Whether this preference is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for updating existing registrar selection preference
/// </summary>
public class UpdateRegistrarSelectionPreferenceDto
{
    /// <summary>
    /// Priority level (lower = higher priority)
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Whether registrar offers hosting services
    /// </summary>
    public bool OffersHosting { get; set; }

    /// <summary>
    /// Whether registrar offers email services
    /// </summary>
    public bool OffersEmail { get; set; }

    /// <summary>
    /// Whether registrar offers SSL certificates
    /// </summary>
    public bool OffersSsl { get; set; }

    /// <summary>
    /// Maximum cost difference for bundling preference
    /// </summary>
    public decimal? MaxCostDifferenceThreshold { get; set; }

    /// <summary>
    /// Prefer for hosting customers
    /// </summary>
    public bool PreferForHostingCustomers { get; set; }

    /// <summary>
    /// Prefer for email customers
    /// </summary>
    public bool PreferForEmailCustomers { get; set; }

    /// <summary>
    /// Whether this preference is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; set; }
}

// ==================== Margin Analysis DTOs ====================

/// <summary>
/// Result of margin analysis calculation
/// </summary>
public class MarginAnalysisResult
{
    /// <summary>
    /// TLD ID
    /// </summary>
    public int TldId { get; set; }

    /// <summary>
    /// TLD extension
    /// </summary>
    public string? TldExtension { get; set; }

    /// <summary>
    /// Registrar ID
    /// </summary>
    public int? RegistrarId { get; set; }

    /// <summary>
    /// Registrar name
    /// </summary>
    public string? RegistrarName { get; set; }

    /// <summary>
    /// Operation type (Registration, Renewal, Transfer)
    /// </summary>
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// Registrar cost
    /// </summary>
    public decimal Cost { get; set; }

    /// <summary>
    /// Customer sales price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Margin amount (Price - Cost)
    /// </summary>
    public decimal MarginAmount { get; set; }

    /// <summary>
    /// Margin percentage
    /// </summary>
    public decimal MarginPercentage { get; set; }

    /// <summary>
    /// Cost currency
    /// </summary>
    public string CostCurrency { get; set; } = "USD";

    /// <summary>
    /// Price currency
    /// </summary>
    public string PriceCurrency { get; set; } = "USD";

    /// <summary>
    /// Whether margin is negative
    /// </summary>
    public bool IsNegativeMargin { get; set; }

    /// <summary>
    /// Whether margin is below threshold
    /// </summary>
    public bool IsLowMargin { get; set; }

    /// <summary>
    /// Alert message if applicable
    /// </summary>
    public string? AlertMessage { get; set; }
}

/// <summary>
/// Request for calculating pricing with discounts
/// </summary>
public class CalculatePricingRequest
{
    /// <summary>
    /// TLD ID
    /// </summary>
    public int TldId { get; set; }

    /// <summary>
    /// Reseller company ID (optional, for discounts)
    /// </summary>
    public int? ResellerCompanyId { get; set; }

    /// <summary>
    /// Operation type (Registration, Renewal, Transfer)
    /// </summary>
    public string OperationType { get; set; } = "Registration";

    /// <summary>
    /// Number of years
    /// </summary>
    public int Years { get; set; } = 1;

    /// <summary>
    /// Whether this is first year (for promotional pricing)
    /// </summary>
    public bool IsFirstYear { get; set; } = true;

    /// <summary>
    /// Target currency for pricing
    /// </summary>
    public string? TargetCurrency { get; set; }
}

/// <summary>
/// Response with calculated pricing
/// </summary>
public class CalculatePricingResponse
{
    /// <summary>
    /// TLD extension
    /// </summary>
    public string? TldExtension { get; set; }

    /// <summary>
    /// Base price before discounts
    /// </summary>
    public decimal BasePrice { get; set; }

    /// <summary>
    /// Discount amount applied
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Final price after discounts
    /// </summary>
    public decimal FinalPrice { get; set; }

    /// <summary>
    /// Currency of the pricing
    /// </summary>
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Whether promotional pricing was applied
    /// </summary>
    public bool IsPromotionalPricing { get; set; }

    /// <summary>
    /// Promotion name if applicable
    /// </summary>
    public string? PromotionName { get; set; }

    /// <summary>
    /// Whether discount was applied
    /// </summary>
    public bool IsDiscountApplied { get; set; }

    /// <summary>
    /// Discount description
    /// </summary>
    public string? DiscountDescription { get; set; }

    /// <summary>
    /// Selected registrar for fulfillment
    /// </summary>
    public int? SelectedRegistrarId { get; set; }

    /// <summary>
    /// Selected registrar name
    /// </summary>
    public string? SelectedRegistrarName { get; set; }
}
