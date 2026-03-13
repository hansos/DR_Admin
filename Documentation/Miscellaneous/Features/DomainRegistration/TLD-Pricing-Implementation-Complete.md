# TLD Temporal Pricing Architecture - Implementation Complete

## ?? Overview

The TLD pricing system has been successfully refactored into a temporal architecture that separates registrar costs from customer sales prices, supports historical tracking, and allows scheduling future price changes.

## ? Implementation Summary

### **Phase 1: Entity Layer - COMPLETED**

Created four new entities with full temporal support:

1. **`RegistrarTldCostPricing`** - Tracks what registrars charge YOU
   - Temporal with `EffectiveFrom`/`EffectiveTo`
   - Per-registrar currency support
   - Special `FirstYearRegistrationCost` for promotions
   - Linked to `RegistrarTld`

2. **`TldSalesPricing`** - Tracks what YOU charge customers
   - Temporal with `EffectiveFrom`/`EffectiveTo`
   - One price per TLD (linked to `TldId`, not `RegistrarTldId`)
   - Special `FirstYearRegistrationPrice` for promotions
   - Promotional pricing flags

3. **`ResellerTldDiscount`** - Customer-specific discounts
   - Temporal with `EffectiveFrom`/`EffectiveTo`
   - Percentage OR fixed amount discounts
   - Operation-specific: `ApplyToRegistration`, `ApplyToRenewal`, `ApplyToTransfer`
   - Linked to `ResellerCompany` and `TldId`

4. **`RegistrarSelectionPreference`** - Registrar selection logic
   - Priority-based selection
   - Bundling preferences (`OffersHosting`, `OffersEmail`, `OffersSsl`)
   - Cost difference threshold for bundling

**Navigation Properties Updated:**
- `RegistrarTld.CostPricingHistory`
- `Tld.SalesPricingHistory` and `Tld.ResellerDiscounts`
- `Registrar.SelectionPreferences`
- `ResellerCompany.TldDiscounts`

### **Phase 2: Configuration & Settings - COMPLETED**

Created `TldPricingSettings` configuration class with:
- **Retention Policies**: `CostPricingRetentionYears` (7), `SalesPricingRetentionYears` (5)
- **Margin Alerts**: `MinimumMarginPercentage` (5%), `EnableNegativeMarginAlerts`, `MarginAlertEmails`
- **Future Editing**: `AllowEditingFuturePrices` (true), `MaxScheduleDays` (365)
- **Currency**: `DefaultSalesCurrency`, `EnableAutoCurrencyConversion`, `CurrencyConversionMarkup`
- **Business Rules**: `AllowDiscountStacking` (false - no stacking promotional + discount)

### **Phase 3: Helper Services - COMPLETED**

1. **`MarginValidator`** - Validates profit margins
   - Checks for negative margins (cost > price)
   - Checks for low margins (below threshold)
   - Generates alert messages
   - Returns email addresses for alerts

2. **`FuturePricingManager`** - Manages future pricing schedules
   - Validates if pricing can be edited (only future)
   - Validates schedule dates (not past, within max days)
   - Determines pricing state (Future, Current, Past)

### **Phase 4: Service Layer - COMPLETED**

**`ITldPricingService`** interface with 30+ methods:

**Cost Pricing Methods:**
- `GetCostPricingHistoryAsync()` - Get all cost pricing for a registrar-TLD
- `GetCurrentCostPricingAsync()` - Get current effective cost pricing
- `GetFutureCostPricingAsync()` - Get scheduled future pricing
- `CreateCostPricingAsync()` - Create new cost pricing (auto-closes existing if EffectiveTo = null)
- `UpdateCostPricingAsync()` - Update future pricing (if allowed)
- `DeleteFutureCostPricingAsync()` - Delete future pricing (if allowed)

**Sales Pricing Methods:**
- `GetSalesPricingHistoryAsync()` - Get all sales pricing for a TLD
- `GetCurrentSalesPricingAsync()` - Get current effective sales pricing
- `GetFutureSalesPricingAsync()` - Get scheduled future pricing
- `CreateSalesPricingAsync()` - Create new sales pricing
- `UpdateSalesPricingAsync()` - Update future pricing
- `DeleteFutureSalesPricingAsync()` - Delete future pricing

**Discount Methods:**
- `GetResellerDiscountsAsync()` - Get all discounts for a reseller company
- `GetCurrentDiscountAsync()` - Get current discount for reseller + TLD
- `CreateDiscountAsync()` - Create new discount
- `UpdateDiscountAsync()` - Update discount
- `DeleteDiscountAsync()` - Delete discount

**Selection Preference Methods:**
- `GetAllSelectionPreferencesAsync()` - Get all registrar preferences
- `GetSelectionPreferenceAsync()` - Get preference for specific registrar
- `CreateSelectionPreferenceAsync()` - Create new preference
- `UpdateSelectionPreferenceAsync()` - Update preference
- `DeleteSelectionPreferenceAsync()` - Delete preference

**Pricing Calculation Methods:**
- `CalculatePricingAsync()` - Calculate final price with discounts/promotions
- `SelectOptimalRegistrarAsync()` - Choose cheapest registrar (with bundling logic)

**Margin Analysis Methods:**
- `CalculateMarginAsync()` - Calculate margin for TLD + operation
- `GetNegativeMarginReportAsync()` - Get all TLDs with negative margins
- `GetLowMarginReportAsync()` - Get all TLDs with low margins

**Currency & Archive Methods:**
- `ConvertCurrencyAsync()` - Convert between currencies using exchange rates
- `ArchiveOldCostPricingAsync()` - Archive old cost pricing (retention policy)
- `ArchiveOldSalesPricingAsync()` - Archive old sales pricing
- `ArchiveOldDiscountsAsync()` - Archive old discounts

**Implementation:** `TldPricingService` and `TldPricingService.Partial` (partial class split for maintainability)

### **Phase 5: DTOs - COMPLETED**

Created comprehensive DTOs for all operations:
- `RegistrarTldCostPricingDto`, `CreateRegistrarTldCostPricingDto`, `UpdateRegistrarTldCostPricingDto`
- `TldSalesPricingDto`, `CreateTldSalesPricingDto`, `UpdateTldSalesPricingDto`
- `ResellerTldDiscountDto`, `CreateResellerTldDiscountDto`, `UpdateResellerTldDiscountDto`
- `RegistrarSelectionPreferenceDto`, `CreateRegistrarSelectionPreferenceDto`, `UpdateRegistrarSelectionPreferenceDto`
- `MarginAnalysisResult`, `CalculatePricingRequest`, `CalculatePricingResponse`

All DTOs include XML documentation for all properties.

### **Phase 6: Controllers - COMPLETED**

1. **`RegistrarTldCostPricingController`** - Manages registrar cost pricing
   - GET `/api/v1/registrar-tld-cost-pricing/registrar-tld/{registrarTldId}` - History
   - GET `/api/v1/registrar-tld-cost-pricing/registrar-tld/{registrarTldId}/current` - Current
   - GET `/api/v1/registrar-tld-cost-pricing/registrar-tld/{registrarTldId}/future` - Future
   - POST `/api/v1/registrar-tld-cost-pricing` - Create
   - PUT `/api/v1/registrar-tld-cost-pricing/{id}` - Update
   - DELETE `/api/v1/registrar-tld-cost-pricing/{id}` - Delete

2. **`TldPricingController`** - Unified pricing operations
   - Sales Pricing CRUD endpoints (`/api/v1/tld-pricing/sales/*`)
   - Pricing Calculation (`POST /api/v1/tld-pricing/calculate`)
   - Margin Analysis (`GET /api/v1/tld-pricing/margin/*`)
   - Archive Management (`POST /api/v1/tld-pricing/archive`)

All endpoints include:
- XML documentation
- Authorization policies
- Proper HTTP status codes
- Error handling with Serilog

### **Phase 7: Database & Configuration - COMPLETED**

**Database Changes:**
- Migration created: `AddTldPricingTables`
- 4 new tables added to `ApplicationDbContext`:
  - `RegistrarTldCostPricing`
  - `TldSalesPricing`
  - `ResellerTldDiscounts`
  - `RegistrarSelectionPreferences`

**Configuration:**
- `appsettings.json` updated with `TldPricing` section
- Services registered in `Program.cs`:
  - `TldPricingSettings` (IOptions)
  - `MarginValidator` (Scoped)
  - `FuturePricingManager` (Scoped)
  - `ITldPricingService` / `TldPricingService` (Transient)

**Build Status:** ? **Build successful** - No compilation errors

---

## ?? Key Features Implemented

### ? **Temporal Pricing**
- Prices have `EffectiveFrom` and `EffectiveTo` dates
- Automatically query current, past, or future pricing
- Schedule future price changes (auto-activate when date passes)
- Complete historical audit trail

### ? **Future Price Editing**
- Can edit/delete future prices before `EffectiveFrom`
- Configurable via `AllowEditingFuturePrices` setting
- Validates schedule dates (not in past, within max days)
- Prevents editing/deleting effective or past prices

### ? **Separation of Concerns**
- **Registrar costs** separate from **sales prices**
- Different currencies per registrar supported
- One sales price per TLD (customers see unified pricing)
- Choose cheapest registrar behind the scenes

### ? **Customer Discounts**
- Linked to `ResellerCompany`
- Percentage OR fixed amount (not both)
- Operation-specific (registration, renewal, transfer)
- Temporal (can expire)
- **No stacking** with promotional pricing (configurable)

### ? **Margin Monitoring**
- Automatic margin calculation
- Negative margin detection (cost > price)
- Low margin alerts (below threshold)
- Email alert configuration
- Margin reports via API endpoints

### ? **Multi-Currency Support**
- Each registrar can have different currency
- Automatic currency conversion using `CurrencyExchangeRate` table
- Optional markup on conversions
- Margin calculations handle currency differences

### ? **Registrar Selection**
- Primary: Choose cheapest registrar
- Secondary: Prefer bundling (hosting, email, SSL)
- Priority-based fallback
- Cost threshold for bundling preference

### ? **Archive Management**
- Configurable retention policies
- Auto-archive old pricing data
- Separate retention for costs (7 years) and sales (5 years)
- Manual archive trigger via API

---

## ?? Data Flow Examples

### **Example 1: Create Future Price Change**

**Scenario:** Schedule .com price increase from $12 to $14 effective March 1, 2025

```csharp
POST /api/v1/tld-pricing/sales
{
  "tldId": 1,
  "effectiveFrom": "2025-03-01T00:00:00Z",
  "effectiveTo": null,
  "registrationPrice": 14.00,
  "renewalPrice": 14.00,
  "transferPrice": 14.00,
  "currency": "USD",
  "isPromotional": false,
  "isActive": true
}
```

**Result:**
- Existing pricing (EffectiveTo = null) automatically closed with `EffectiveTo = 2025-02-28T23:59:59Z`
- New pricing created with `EffectiveFrom = 2025-03-01`
- Feb 28: Customers pay $12.00
- Mar 1: Customers pay $14.00 (automatic switch)

### **Example 2: Calculate Pricing with Discount**

**Scenario:** Customer (Reseller #5) wants to register .com for 1 year

```csharp
POST /api/v1/tld-pricing/calculate
{
  "tldId": 1,
  "resellerCompanyId": 5,
  "operationType": "Registration",
  "years": 1,
  "isFirstYear": true,
  "targetCurrency": "USD"
}
```

**Response:**
```json
{
  "tldExtension": "com",
  "basePrice": 12.00,
  "discountAmount": 1.20,
  "finalPrice": 10.80,
  "currency": "USD",
  "isPromotionalPricing": false,
  "isDiscountApplied": true,
  "discountDescription": "10% discount",
  "selectedRegistrarId": 3,
  "selectedRegistrarName": "Namecheap"
}
```

### **Example 3: Margin Analysis**

**Scenario:** Check margin for .com registration

```csharp
GET /api/v1/tld-pricing/margin/tld/1?operationType=Registration
```

**Response:**
```json
{
  "tldId": 1,
  "tldExtension": "com",
  "registrarId": 3,
  "registrarName": "Namecheap",
  "operationType": "Registration",
  "cost": 8.50,
  "price": 12.00,
  "marginAmount": 3.50,
  "marginPercentage": 41.18,
  "costCurrency": "USD",
  "priceCurrency": "USD",
  "isNegativeMargin": false,
  "isLowMargin": false,
  "alertMessage": null
}
```

---

## ?? Migration from Old System

### **Step 1: Review Current Data**
```sql
SELECT COUNT(*) FROM RegistrarTlds;
-- Understand how many pricing records exist
```

### **Step 2: Apply Migration**
```bash
cd DR_Admin
dotnet ef database update
```

### **Step 3: Migrate Existing Cost Data**
```csharp
// Service method to migrate old RegistrarTld costs to RegistrarTldCostPricing
var registrarTlds = await _context.RegistrarTlds.ToListAsync();

foreach (var rt in registrarTlds)
{
    var costPricing = new RegistrarTldCostPricing
    {
        RegistrarTldId = rt.Id,
        EffectiveFrom = rt.CreatedAt,
        EffectiveTo = null, // Current/active
        RegistrationCost = rt.RegistrationCost,
        RenewalCost = rt.RenewalCost,
        TransferCost = rt.TransferCost,
        PrivacyCost = rt.PrivacyCost,
        Currency = rt.Currency,
        IsActive = rt.IsActive,
        CreatedBy = "System Migration"
    };
    
    _context.RegistrarTldCostPricing.Add(costPricing);
}

await _context.SaveChangesAsync();
```

### **Step 4: Migrate Existing Sales Prices**
```csharp
// Take first registrar's price for each TLD as sales price
var tlds = await _context.Tlds.ToListAsync();

foreach (var tld in tlds)
{
    var firstRegistrarTld = await _context.RegistrarTlds
        .Where(rt => rt.TldId == tld.Id && rt.IsActive)
        .OrderBy(rt => rt.RegistrationPrice)
        .FirstOrDefaultAsync();
    
    if (firstRegistrarTld != null)
    {
        var salesPricing = new TldSalesPricing
        {
            TldId = tld.Id,
            EffectiveFrom = firstRegistrarTld.CreatedAt,
            EffectiveTo = null,
            RegistrationPrice = firstRegistrarTld.RegistrationPrice,
            RenewalPrice = firstRegistrarTld.RenewalPrice,
            TransferPrice = firstRegistrarTld.TransferPrice,
            PrivacyPrice = firstRegistrarTld.PrivacyPrice,
            Currency = firstRegistrarTld.Currency,
            IsPromotional = false,
            IsActive = true,
            CreatedBy = "System Migration"
        };
        
        _context.TldSalesPricing.Add(salesPricing);
    }
}

await _context.SaveChangesAsync();
```

### **Step 5: Verify Migration**
```sql
SELECT 
    COUNT(DISTINCT TldId) AS TLDsWithSalesPricing
FROM TldSalesPricing
WHERE IsActive = 1;

SELECT 
    COUNT(DISTINCT RegistrarTldId) AS RegistrarTldsWithCostPricing
FROM RegistrarTldCostPricing
WHERE IsActive = 1;
```

### **Step 6: Optional - Keep Old Fields for Comparison**
The old `RegistrarTld` fields (`RegistrationCost`, `RegistrationPrice`, etc.) can remain temporarily for data comparison and validation. Once confident in the new system, they can be removed via a future migration.

---

## ?? Configuration Example

**appsettings.json:**
```json
{
  "TldPricing": {
    "CostPricingRetentionYears": 7,
    "SalesPricingRetentionYears": 5,
    "DiscountHistoryRetentionYears": 5,
    "MinimumMarginPercentage": 5.0,
    "EnableNegativeMarginAlerts": true,
    "EnableLowMarginAlerts": true,
    "MarginAlertEmails": "pricing@yourcompany.com,admin@yourcompany.com",
    "AllowEditingFuturePrices": true,
    "MaxScheduleDays": 365,
    "DefaultSalesCurrency": "USD",
    "EnableAutoCurrencyConversion": true,
    "CurrencyConversionMarkup": 0.0,
    "CurrentPriceCacheMinutes": 60,
    "AllowDiscountStacking": false,
    "EnableAutoArchiving": true,
    "AutoArchivingHour": 2
  }
}
```

---

## ?? Authorization Policies

All endpoints use role-based authorization:
- **`Pricing.Read`** - View pricing data
- **`Pricing.Write`** - Create/update pricing
- **`Pricing.Delete`** - Delete future pricing
- **`Pricing.Admin`** - Archive management

Ensure these policies are configured in your authorization setup.

---

## ?? Related Documentation

- **TLD-Pricing-Migration-Guide.md** - Detailed migration steps
- **TLD-Pricing-Quick-Reference.md** - API endpoint quick reference
- **TLD-Pricing-Diagrams.md** - Architecture diagrams and ERD
- **TLD-Pricing-Checklist.md** - Implementation checklist

---

## ? Implementation Checklist

- [x] Entity classes created with navigation properties
- [x] Configuration class created (`TldPricingSettings`)
- [x] Helper services created (`MarginValidator`, `FuturePricingManager`)
- [x] Service interface created (`ITldPricingService`)
- [x] Service implementation created (`TldPricingService` + partial)
- [x] DTOs created with XML documentation
- [x] Controllers created with XML documentation
- [x] Database migration created
- [x] Services registered in `Program.cs`
- [x] Configuration added to `appsettings.json`
- [x] Build successful with no errors
- [ ] Data migration script created (optional)
- [ ] Integration tests written (optional)
- [ ] Background job for auto-archiving (optional)

---

## ?? Next Steps

1. **Run Migration:** `dotnet ef database update` in DR_Admin project
2. **Migrate Data:** Use examples above to migrate existing `RegistrarTld` data
3. **Test Endpoints:** Use Swagger or Postman to test pricing endpoints
4. **Configure Alerts:** Set `MarginAlertEmails` in appsettings
5. **Monitor Margins:** Check `/api/v1/tld-pricing/margin/negative` regularly
6. **Schedule Archive:** Set up background job to call archive endpoint

---

**Implementation Status:** ? **COMPLETE**  
**Build Status:** ? **Successful**  
**Ready for:** Testing and Data Migration
