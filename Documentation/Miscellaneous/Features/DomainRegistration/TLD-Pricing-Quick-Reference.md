# TLD Pricing - Quick Reference Card

## ? Finalized Design Decisions

| Question | Answer | Implementation |
|----------|--------|----------------|
| **Edit Future Prices?** | ? YES | `UpdateFutureSalesPricingAsync()` / `UpdateFutureCostPricingAsync()` |
| **Multi-Year Domains?** | ? No Issue | Price locked at purchase time, future changes don't affect |
| **Archive Policy?** | ? Configurable | appsettings: `CostPricingRetentionYears` (7), `SalesPricingRetentionYears` (5) |
| **Negative Margin Alerts?** | ? Absolutely | Automatic email alerts, configurable threshold |
| **Currency Conversion?** | ? Automatic | Uses `CurrencyExchangeRate` table, optional markup |

---

## ?? Files Created

### **Entity Classes** (in `DR_Admin/Data/Entities/`)
- ? `RegistrarTldCostPricing.cs` - Registrar costs (temporal)
- ? `TldSalesPricing.cs` - Customer prices (temporal)
- ? `ResellerTldDiscount.cs` - Customer discounts
- ? `RegistrarSelectionPreference.cs` - Registrar selection logic

### **Configuration** (in `DR_Admin/Configuration/`)
- ? `TldPricingSettings.cs` - AppSettings configuration class

### **Services** (in `DR_Admin/Services/`)
- ? `ITldPricingService.cs` - Original interface (17 methods)
- ? `ITldPricingService.Updated.cs` - Enhanced interface (25 methods)
- ? `MarginValidator.cs` - Margin validation and alerts
- ? `FuturePricingManager.cs` - Future price editing logic

### **DTOs** (in `DR_Admin/DTOs/`)
- ? `TldPricingDtos.cs` - All pricing DTOs (9 types)

### **Documentation** (in `Documentation/`)
- ? `TLD-Pricing-Migration-Guide.md` - Complete migration plan
- ? `TLD-Pricing-Summary.md` - Architecture overview
- ? `TLD-Pricing-Diagrams.md` - Visual diagrams and flows
- ? `TLD-Pricing-Checklist.md` - Implementation checklist
- ? `TLD-Pricing-Implementation-Final.md` - Final implementation guide
- ? `appsettings.TldPricing.example.json` - Configuration example

---

## ?? Key Configuration (appsettings.json)

```json
{
  "TldPricing": {
    "CostPricingRetentionYears": 7,
    "SalesPricingRetentionYears": 5,
    "MinimumMarginPercentage": 5.0,
    "EnableNegativeMarginAlerts": true,
    "MarginAlertEmails": "pricing@company.com,admin@company.com",
    "AllowEditingFuturePrices": true,
    "EnableAutoCurrencyConversion": true
  }
}
```

---

## ?? Quick Start

### **1. Create Database Migration**
```bash
dotnet ef migrations add AddTldPricingTables
dotnet ef database update
```

### **2. Configure Services (Program.cs)**
```csharp
builder.Services.Configure<TldPricingSettings>(
    builder.Configuration.GetSection(TldPricingSettings.SectionName));

builder.Services.AddScoped<MarginValidator>();
builder.Services.AddScoped<FuturePricingManager>();
builder.Services.AddScoped<ITldPricingService, TldPricingService>();
```

### **3. Migrate Existing Data**
```sql
-- Migrate costs
INSERT INTO RegistrarTldCostPricing (...)
SELECT ... FROM RegistrarTld;

-- Migrate sales prices
INSERT INTO TldSalesPricing (...)
SELECT DISTINCT ON (TldId) ... FROM RegistrarTld;
```

---

## ?? Common Operations

### **Schedule Future Price Change**
```csharp
var newPricing = new TldSalesPricing
{
    TldId = 1,
    EffectiveFrom = new DateTime(2025, 3, 1),
    RegistrationPrice = 14.00m,
    Currency = "USD"
};

await _pricingService.ScheduleSalesPriceChangeAsync(
    tldId: 1,
    effectiveFrom: new DateTime(2025, 3, 1),
    newPricing: newPricing,
    createdBy: "admin@example.com"
);
```

### **Edit Scheduled Price (before it goes live)**
```csharp
await _pricingService.UpdateFutureSalesPricingAsync(
    salesPricingId: 123,
    updatedPricing: new TldSalesPricing 
    { 
        RegistrationPrice = 13.50m // Changed from 14.00
    },
    modifiedBy: "admin@example.com"
);
```

### **Check for Negative Margins**
```csharp
var marginReport = await _pricingService.GetNegativeMarginReportAsync();

foreach (var margin in marginReport)
{
    Console.WriteLine($"{margin.TldExtension}: Margin = {margin.RegistrationMargin:C}");
    // If negative, alerts already sent to configured emails
}
```

### **Get Effective Price for Customer**
```csharp
var price = await _pricingService.CalculateEffectivePriceAsync(
    tldId: 1,
    resellerCompanyId: 123, // For discount lookup
    operationType: "Registration",
    isFirstYear: true,
    currency: "USD"
);

// Returns final price with discounts/promotions applied
```

---

## ?? Multi-Year Domain Pricing

**Key Rule**: Price is locked at purchase time for entire period.

```csharp
// Customer buys example.com for 3 years
var purchase = await ProcessDomainPurchase(
    domain: "example.com",
    years: 3,
    customerId: 123
);

// Prices locked at purchase:
// - Customer pays: $7.99 (year 1) + $12 (year 2) + $12 (year 3) = $31.99
// - Registrar charges: $7.50 (year 1) + $8 (year 2) + $8 (year 3) = $23.50
// - Margin: $8.49

// Future price changes (Mar 1, 2025: $14/year) DO NOT affect this registration
// Only renewals in 2028 will use new pricing
```

---

## ?? Margin Alerts

Automatic alerts sent when:

| Condition | Threshold | Example |
|-----------|-----------|---------|
| **Negative Margin** | Margin < 0% | Cost: $10, Price: $9 ? Alert! |
| **Low Margin** | Margin < 5% | Cost: $10, Price: $10.40 ? Alert! |

**Email Configuration**:
- Recipients: From `MarginAlertEmails` in appsettings
- Trigger: On `CalculateMarginAsync()` call
- Content: Detailed margin breakdown with recommendations

---

## ?? Currency Conversion

**Automatic Conversion** for margin calculations:

```csharp
// Registrar charges in EUR
var costInEur = 8.50m;

// You sell in USD
var priceInUsd = 12.00m;

// Margin calculation auto-converts
var margin = await _pricingService.CalculateMarginAsync(
    tldId: 1,
    registrarTldId: 5,
    targetCurrency: "USD" // Forces conversion
);

// Result uses latest exchange rate from CurrencyExchangeRate table
// Optional markup applied if configured
```

**Exchange Rate Source**: Uses your existing `CurrencyExchangeRate` table populated by `ExchangeRateLib`

---

## ?? Archive Policy

**Configurable Retention**:
- Cost Pricing: 7 years (tax compliance)
- Sales Pricing: 5 years
- Discounts: 5 years

**Scheduled Job** (recommended: monthly):
```csharp
// Run archival job
var archivedCount = await _pricingService.ArchiveOldPricingDataAsync();

_logger.Information("Archived {Count} old pricing records", archivedCount);
```

---

## ?? Business Rules

| Rule | Enforcement |
|------|-------------|
| **No Discount Stacking** | Promo XOR Customer Discount (best deal wins) |
| **Future Price Editing** | Only if `EffectiveFrom > DateTime.UtcNow` |
| **Multi-Year Lock** | Price frozen at purchase, unaffected by future changes |
| **Negative Margin** | Alert sent, but not blocked (may have strategic reasons) |
| **Temporal Integrity** | No overlapping date ranges, EffectiveFrom < EffectiveTo |

---

## ?? Troubleshooting

| Issue | Solution |
|-------|----------|
| **"Cannot edit pricing"** | Check `EffectiveFrom` - must be in future |
| **No margin alerts** | Check `MarginAlertEmails` in appsettings |
| **Currency conversion fails** | Ensure `CurrencyExchangeRate` data exists |
| **No current pricing** | Query `WHERE EffectiveTo IS NULL` |
| **Multi-year price wrong** | Check price locked at purchase time in domain record |

---

## ?? Support

- **Migration Guide**: `Documentation/TLD-Pricing-Migration-Guide.md`
- **Implementation Guide**: `Documentation/TLD-Pricing-Implementation-Final.md`
- **Architecture Diagrams**: `Documentation/TLD-Pricing-Diagrams.md`
- **Checklist**: `Documentation/TLD-Pricing-Checklist.md`

---

## ? Next Step

```bash
# Create the database migration
dotnet ef migrations add AddTldPricingTables
```

Then review the migration file and proceed with Phase 2 (data migration).

**Estimated Total Time**: 9-12 days

Good luck! ??
