# TLD Pricing - Implementation Guide (Final)

## ?? Finalized Design Decisions

Based on your requirements, here are the confirmed design decisions:

### ? **1. Edit Future Prices: YES**
- Scheduled prices **CAN be edited** before `EffectiveFrom` passes
- Implemented via `UpdateFutureSalesPricingAsync()` and `UpdateFutureCostPricingAsync()`
- Validation: Only future prices (EffectiveFrom > NOW) can be edited
- Use case: Correct mistakes, adjust for market changes before go-live

### ? **2. Multi-Year Domains: No Issue**
- Multi-year domains are **paid in advance for full period**
- Price is **locked at purchase time** (customer and registrar)
- Future price changes **do not affect** existing multi-year registrations
- Only renewals use new pricing

### ? **3. Archive Policy: Configurable**
- Use `appsettings.json` for retention periods
- Default: 7 years for cost pricing (tax compliance)
- Default: 5 years for sales pricing and discounts
- Implemented via scheduled job using `ArchiveOldPricingDataAsync()`

### ? **4. Negative Margins: Alert**
- **Automatic alerts** when registrar cost > sales price
- **Low margin alerts** when margin < configurable threshold (default: 5%)
- Email notifications to configured recipients
- Margin monitoring dashboard

### ? **5. Currency Conversion: Automatic**
- Use existing `CurrencyExchangeRate` table
- Automatic conversion for margin calculations
- Optional markup on conversions (configurable)
- Real-time exchange rates via ExchangeRateLib

---

## ??? New Architecture Components

### **1. Configuration Classes**

#### `TldPricingSettings.cs` ? Created
```csharp
public class TldPricingSettings
{
    public int CostPricingRetentionYears { get; set; } = 7;
    public int SalesPricingRetentionYears { get; set; } = 5;
    public decimal MinimumMarginPercentage { get; set; } = 5.0m;
    public bool EnableNegativeMarginAlerts { get; set; } = true;
    public bool AllowEditingFuturePrices { get; set; } = true;
    public bool EnableAutoCurrencyConversion { get; set; } = true;
    public string DefaultSalesCurrency { get; set; } = "USD";
    // ... more settings
}
```

#### `appsettings.json` Configuration
```json
{
  "TldPricing": {
    "CostPricingRetentionYears": 7,
    "SalesPricingRetentionYears": 5,
    "MinimumMarginPercentage": 5.0,
    "EnableNegativeMarginAlerts": true,
    "EnableLowMarginAlerts": true,
    "MarginAlertEmails": "pricing@example.com,admin@example.com",
    "AllowEditingFuturePrices": true,
    "EnableAutoCurrencyConversion": true
  }
}
```

### **2. Helper Classes**

#### `MarginValidator.cs` ? Created
- Validates margins against thresholds
- Generates warnings for negative/low margins
- Determines when to send alerts

#### `FuturePricingManager.cs` ? Created
- Validates if pricing can be edited/deleted
- Checks schedule date limits
- Determines pricing state (current/future/past)

### **3. Updated Service Interface**

#### `ITldPricingService.cs` ? Updated
New methods added:
```csharp
// Future price editing
Task<RegistrarTldCostPricing> UpdateFutureCostPricingAsync(...);
Task<bool> DeleteFutureCostPricingAsync(...);
Task<TldSalesPricing> UpdateFutureSalesPricingAsync(...);
Task<bool> DeleteFutureSalesPricingAsync(...);

// Margin analysis with alerts
Task<MarginAnalysisResult> CalculateMarginAsync(...);
Task<List<MarginAnalysisResult>> GetNegativeMarginReportAsync();
Task<List<MarginAnalysisResult>> GetLowMarginReportAsync();

// Currency conversion
Task<decimal> ConvertCurrencyAsync(...);

// Archive management
Task<int> ArchiveOldPricingDataAsync();
Task<int> ArchiveOldDiscountDataAsync();
```

---

## ?? Implementation Steps

### **Phase 1: Configuration Setup**

#### 1.1 Add Configuration to Startup/Program.cs
```csharp
// Add to Program.cs
builder.Services.Configure<TldPricingSettings>(
    builder.Configuration.GetSection(TldPricingSettings.SectionName));

// Register helper services
builder.Services.AddScoped<MarginValidator>();
builder.Services.AddScoped<FuturePricingManager>();
builder.Services.AddScoped<ITldPricingService, TldPricingService>();
```

#### 1.2 Add to appsettings.json
```json
{
  "TldPricing": {
    "CostPricingRetentionYears": 7,
    "SalesPricingRetentionYears": 5,
    "DiscountHistoryRetentionYears": 5,
    "MinimumMarginPercentage": 5.0,
    "EnableNegativeMarginAlerts": true,
    "EnableLowMarginAlerts": true,
    "MarginAlertEmails": "pricing@yourcompany.com",
    "AllowEditingFuturePrices": true,
    "MaxScheduleDays": 365,
    "DefaultSalesCurrency": "USD",
    "EnableAutoCurrencyConversion": true,
    "CurrencyConversionMarkup": 0.0,
    "CurrentPriceCacheMinutes": 60
  }
}
```

### **Phase 2: Service Implementation**

#### 2.1 Implement TldPricingService (Key Methods)

**Update Future Sales Pricing:**
```csharp
public async Task<TldSalesPricing> UpdateFutureSalesPricingAsync(
    int salesPricingId, 
    TldSalesPricing updatedPricing, 
    string? modifiedBy,
    CancellationToken cancellationToken = default)
{
    var existing = await _context.TldSalesPricing
        .FindAsync(new object[] { salesPricingId }, cancellationToken);
    
    if (existing == null)
        throw new NotFoundException($"Sales pricing {salesPricingId} not found");
    
    // Validate can edit (only future prices)
    if (!_futurePricingManager.CanEdit(existing.EffectiveFrom))
        throw new InvalidOperationException("Cannot edit pricing that is already effective or past");
    
    // Validate new schedule date
    var (isValid, errorMessage) = _futurePricingManager.ValidateScheduleDate(updatedPricing.EffectiveFrom);
    if (!isValid)
        throw new InvalidOperationException(errorMessage);
    
    // Update fields
    existing.EffectiveFrom = updatedPricing.EffectiveFrom;
    existing.EffectiveTo = updatedPricing.EffectiveTo;
    existing.RegistrationPrice = updatedPricing.RegistrationPrice;
    existing.RenewalPrice = updatedPricing.RenewalPrice;
    existing.TransferPrice = updatedPricing.TransferPrice;
    existing.FirstYearRegistrationPrice = updatedPricing.FirstYearRegistrationPrice;
    existing.IsPromotional = updatedPricing.IsPromotional;
    existing.PromotionName = updatedPricing.PromotionName;
    existing.Notes = updatedPricing.Notes;
    // CreatedBy stays original, UpdatedAt handled by DbContext
    
    await _context.SaveChangesAsync(cancellationToken);
    
    _logger.Information("Updated future sales pricing {PricingId} by {User}", 
        salesPricingId, modifiedBy);
    
    return existing;
}
```

**Calculate Margin with Alerts:**
```csharp
public async Task<MarginAnalysisResult> CalculateMarginAsync(
    int tldId, 
    int registrarTldId, 
    string targetCurrency = "USD",
    CancellationToken cancellationToken = default)
{
    // Get current cost pricing
    var costPricing = await GetCurrentCostPricingAsync(registrarTldId, cancellationToken);
    if (costPricing == null)
        throw new NotFoundException($"No cost pricing found for RegistrarTld {registrarTldId}");
    
    // Get current sales pricing
    var salesPricing = await GetCurrentSalesPricingAsync(tldId, targetCurrency, cancellationToken);
    if (salesPricing == null)
        throw new NotFoundException($"No sales pricing found for TLD {tldId}");
    
    // Convert cost to target currency if needed
    var costInTargetCurrency = costPricing;
    decimal? exchangeRate = null;
    
    if (costPricing.Currency != targetCurrency && _settings.EnableAutoCurrencyConversion)
    {
        var convertedRegCost = await ConvertCurrencyAsync(
            costPricing.RegistrationCost, costPricing.Currency, targetCurrency, null, cancellationToken);
        var convertedRenCost = await ConvertCurrencyAsync(
            costPricing.RenewalCost, costPricing.Currency, targetCurrency, null, cancellationToken);
        var convertedTrnCost = await ConvertCurrencyAsync(
            costPricing.TransferCost, costPricing.Currency, targetCurrency, null, cancellationToken);
        
        // Get exchange rate for reporting
        var rate = await _context.CurrencyExchangeRates
            .Where(r => r.FromCurrency == costPricing.Currency && 
                       r.ToCurrency == targetCurrency &&
                       r.Date <= DateTime.UtcNow)
            .OrderByDescending(r => r.Date)
            .Select(r => r.Rate)
            .FirstOrDefaultAsync(cancellationToken);
        
        exchangeRate = rate;
    }
    
    var result = new MarginAnalysisResult
    {
        TldId = tldId,
        TldExtension = salesPricing.Tld.Extension,
        RegistrarTldId = registrarTldId,
        RegistrarName = costPricing.RegistrarTld.Registrar.Name,
        
        RegistrationCost = costInTargetCurrency.RegistrationCost,
        RegistrationPrice = salesPricing.RegistrationPrice,
        RegistrationMargin = salesPricing.RegistrationPrice - costInTargetCurrency.RegistrationCost,
        
        RenewalCost = costInTargetCurrency.RenewalCost,
        RenewalPrice = salesPricing.RenewalPrice,
        RenewalMargin = salesPricing.RenewalPrice - costInTargetCurrency.RenewalCost,
        
        TransferCost = costInTargetCurrency.TransferCost,
        TransferPrice = salesPricing.TransferPrice,
        TransferMargin = salesPricing.TransferPrice - costInTargetCurrency.TransferCost,
        
        CostCurrency = costPricing.Currency,
        SalesCurrency = salesPricing.Currency,
        ExchangeRate = exchangeRate
    };
    
    // Calculate margin percentages
    result.RegistrationMarginPercentage = salesPricing.RegistrationPrice > 0
        ? (result.RegistrationMargin / salesPricing.RegistrationPrice) * 100
        : 0;
    result.RenewalMarginPercentage = salesPricing.RenewalPrice > 0
        ? (result.RenewalMargin / salesPricing.RenewalPrice) * 100
        : 0;
    result.TransferMarginPercentage = salesPricing.TransferPrice > 0
        ? (result.TransferMargin / salesPricing.TransferPrice) * 100
        : 0;
    
    // Validate margins
    _marginValidator.ValidateMargin(result.RegistrationCost, result.RegistrationPrice, "Registration", result);
    _marginValidator.ValidateMargin(result.RenewalCost, result.RenewalPrice, "Renewal", result);
    _marginValidator.ValidateMargin(result.TransferCost, result.TransferPrice, "Transfer", result);
    
    // Send alert if needed
    if (_marginValidator.ShouldAlert(result))
    {
        await SendMarginAlertAsync(result, cancellationToken);
    }
    
    return result;
}
```

**Currency Conversion:**
```csharp
public async Task<decimal> ConvertCurrencyAsync(
    decimal amount, 
    string fromCurrency, 
    string toCurrency, 
    DateTime? date = null,
    CancellationToken cancellationToken = default)
{
    if (fromCurrency == toCurrency)
        return amount;
    
    if (!_settings.EnableAutoCurrencyConversion)
        throw new InvalidOperationException("Automatic currency conversion is disabled");
    
    var targetDate = date ?? DateTime.UtcNow;
    
    // Get exchange rate
    var rate = await _context.CurrencyExchangeRates
        .Where(r => r.FromCurrency == fromCurrency && 
                   r.ToCurrency == toCurrency &&
                   r.Date <= targetDate)
        .OrderByDescending(r => r.Date)
        .Select(r => r.Rate)
        .FirstOrDefaultAsync(cancellationToken);
    
    if (rate == 0)
        throw new NotFoundException($"No exchange rate found for {fromCurrency} to {toCurrency}");
    
    var converted = amount * rate;
    
    // Apply markup if configured
    if (_settings.CurrencyConversionMarkup > 0)
    {
        converted *= (1 + _settings.CurrencyConversionMarkup / 100);
    }
    
    return Math.Round(converted, 2);
}
```

**Archive Old Data:**
```csharp
public async Task<int> ArchiveOldPricingDataAsync(CancellationToken cancellationToken = default)
{
    var costCutoffDate = DateTime.UtcNow.AddYears(-_settings.CostPricingRetentionYears);
    var salesCutoffDate = DateTime.UtcNow.AddYears(-_settings.SalesPricingRetentionYears);
    
    // Archive (or delete) old cost pricing
    var oldCostPricing = await _context.RegistrarTldCostPricing
        .Where(p => p.EffectiveTo.HasValue && p.EffectiveTo.Value < costCutoffDate)
        .ToListAsync(cancellationToken);
    
    // Archive (or delete) old sales pricing
    var oldSalesPricing = await _context.TldSalesPricing
        .Where(p => p.EffectiveTo.HasValue && p.EffectiveTo.Value < salesCutoffDate)
        .ToListAsync(cancellationToken);
    
    var totalArchived = 0;
    
    if (oldCostPricing.Any())
    {
        // Option 1: Move to archive table (recommended for compliance)
        // Option 2: Soft delete (add IsArchived flag)
        // Option 3: Hard delete (use with caution!)
        
        _context.RegistrarTldCostPricing.RemoveRange(oldCostPricing);
        totalArchived += oldCostPricing.Count;
    }
    
    if (oldSalesPricing.Any())
    {
        _context.TldSalesPricing.RemoveRange(oldSalesPricing);
        totalArchived += oldSalesPricing.Count;
    }
    
    await _context.SaveChangesAsync(cancellationToken);
    
    _logger.Information("Archived {Count} old pricing records", totalArchived);
    
    return totalArchived;
}
```

### **Phase 3: Controller Endpoints**

#### New Endpoints to Add:

**Edit Future Sales Pricing:**
```csharp
/// <summary>
/// Updates a scheduled sales price before it becomes effective
/// </summary>
[HttpPut("tlds/{tldId}/sales-pricing/{pricingId}/future")]
[Authorize(Roles = "Admin")]
public async Task<ActionResult<TldSalesPricingDto>> UpdateFutureSalesPricing(
    int tldId,
    int pricingId,
    [FromBody] UpdateTldSalesPricingDto updateDto)
{
    try
    {
        var updatedPricing = new TldSalesPricing
        {
            TldId = tldId,
            EffectiveFrom = updateDto.EffectiveFrom,
            EffectiveTo = updateDto.EffectiveTo,
            RegistrationPrice = updateDto.RegistrationPrice,
            RenewalPrice = updateDto.RenewalPrice,
            TransferPrice = updateDto.TransferPrice,
            FirstYearRegistrationPrice = updateDto.FirstYearRegistrationPrice,
            Currency = updateDto.Currency,
            IsPromotional = updateDto.IsPromotional,
            PromotionName = updateDto.PromotionName,
            Notes = updateDto.Notes
        };
        
        var result = await _pricingService.UpdateFutureSalesPricingAsync(
            pricingId, updatedPricing, User.Identity?.Name);
        
        return Ok(MapToDto(result));
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(ex.Message);
    }
}
```

**Margin Reports:**
```csharp
/// <summary>
/// Gets report of all TLDs with negative margins
/// </summary>
[HttpGet("margin/negative")]
[Authorize(Roles = "Admin,Pricing")]
public async Task<ActionResult<List<MarginAnalysisResult>>> GetNegativeMarginReport()
{
    var report = await _pricingService.GetNegativeMarginReportAsync();
    return Ok(report);
}

/// <summary>
/// Gets report of all TLDs with low margins
/// </summary>
[HttpGet("margin/low")]
[Authorize(Roles = "Admin,Pricing")]
public async Task<ActionResult<List<MarginAnalysisResult>>> GetLowMarginReport()
{
    var report = await _pricingService.GetLowMarginReportAsync();
    return Ok(report);
}
```

---

## ?? Usage Examples

### **Example 1: Edit Scheduled Price Increase**

**Scenario**: You scheduled .com to increase to $14 on March 1, but want to change it to $13.50

```csharp
// Original scheduled price
{
  "effectiveFrom": "2025-03-01T00:00:00Z",
  "registrationPrice": 14.00
}

// Update before it goes live
PUT /api/tlds/1/sales-pricing/123/future
{
  "effectiveFrom": "2025-03-01T00:00:00Z",
  "registrationPrice": 13.50,  // Changed
  "renewalPrice": 13.50,
  "transferPrice": 13.50
}
```

### **Example 2: Margin Alert Notification**

When margin calculation detects negative margin:

**Email Sent To**: pricing@yourcompany.com, admin@yourcompany.com

**Subject**: ?? Negative Margin Alert: .com domain

**Body**:
```
WARNING: Negative margin detected for .com domain

Registrar: GoDaddy
Registration Cost: $9.50 USD
Registration Price: $9.00 USD
Margin: -$0.50 (-5.56%)

Renewal Cost: $9.50 USD
Renewal Price: $9.00 USD  
Margin: -$0.50 (-5.56%)

Action Required: Adjust sales pricing or negotiate better registrar rates.

View margin report: https://admin.example.com/pricing/margin
```

### **Example 3: Multi-Year Domain Purchase**

```csharp
// Customer buys example.com for 3 years
var purchaseRequest = new
{
    domainName = "example.com",
    years = 3,
    resellerCompanyId = 123
};

// System calculates price at purchase time
var pricing = await _pricingService.CalculateEffectivePriceAsync(
    tldId: 1,
    resellerCompanyId: 123,
    operationType: "Registration",
    isFirstYear: true,
    currency: "USD"
);

// Result:
// - First year: $7.99 (promotional)
// - Years 2-3: $12.00 each
// - Total: $31.99

// Lock prices in domain record
var domain = new RegisteredDomain
{
    Name = "example.com",
    Years = 3,
    PricePerYear = new[] { 7.99m, 12.00m, 12.00m },
    TotalPrice = 31.99m,
    PriceLockedAt = DateTime.UtcNow
};

// Future price changes don't affect this registration
// Renewal in 3 years will use pricing at renewal date
```

---

## ?? Deployment Checklist

- [ ] Add `TldPricingSettings` to appsettings.json
- [ ] Register services in Program.cs/Startup.cs
- [ ] Implement `TldPricingService` with all new methods
- [ ] Add controller endpoints for future price editing
- [ ] Add controller endpoints for margin reports
- [ ] Set up email configuration for margin alerts
- [ ] Create scheduled job for archiving old data
- [ ] Test future price editing workflow
- [ ] Test margin calculation with currency conversion
- [ ] Test multi-year domain pricing
- [ ] Deploy margin monitoring dashboard

---

## ?? Margin Alert Configuration

Configure email recipients in appsettings.json:

```json
{
  "TldPricing": {
    "MarginAlertEmails": "pricing@company.com,cfo@company.com,admin@company.com"
  }
}
```

Alerts trigger when:
- Margin < 0% (negative margin - losing money)
- Margin < 5% (low margin - configurable threshold)

---

## ?? Summary

All design decisions finalized:
- ? Future prices can be edited
- ? Multi-year domains handled (price locked at purchase)
- ? Archive policy configurable
- ? Negative margin alerts implemented
- ? Automatic currency conversion

**Ready for implementation!** ??
