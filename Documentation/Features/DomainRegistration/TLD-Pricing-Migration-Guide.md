# TLD Pricing Refactoring - Migration Guide

## ?? Overview

This document outlines the migration from storing prices directly in `RegistrarTld` to a temporal pricing system with separate cost and sales pricing tables.

## ?? Goals

1. **Separate Concerns**: Split registrar costs from customer sales prices
2. **Historical Tracking**: Maintain complete price change history for auditing (7 years for costs, 5 years for sales)
3. **Future Scheduling**: Schedule price changes in advance (auto-activation when EffectiveFrom passes)
4. **Multi-Currency**: Support different currencies per registrar
5. **Customer Discounts**: Link discounts to ResellerCompany (no stacking with promotions)
6. **Smart Registrar Selection**: Choose cheapest registrar, but prefer bundling with hosting/email providers

## ?? New Table Structure

### **RegistrarTldCostPricing**
Stores what registrars charge you (your costs).

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| RegistrarTldId | int | FK to RegistrarTld |
| EffectiveFrom | DateTime | When pricing starts (UTC) |
| EffectiveTo | DateTime? | When pricing ends (NULL = current) |
| RegistrationCost | decimal | Standard registration cost/year |
| RenewalCost | decimal | Renewal cost/year |
| TransferCost | decimal | Transfer cost |
| PrivacyCost | decimal? | Privacy protection cost/year |
| FirstYearRegistrationCost | decimal? | Special first-year promo cost |
| Currency | string | ISO 4217 (USD, EUR, etc.) |
| Notes | string? | Additional information |
| CreatedBy | string? | User who created this entry |
| CreatedAt | DateTime | Auto-populated |
| UpdatedAt | DateTime | Auto-populated |

**Indexes:**
- `IX_RegistrarTldCostPricing_RegistrarTldId_EffectiveFrom`
- `IX_RegistrarTldCostPricing_EffectiveTo`

### **TldSalesPricing**
Stores what you charge customers (your sales prices).

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| TldId | int | FK to Tld (NOT RegistrarTldId) |
| EffectiveFrom | DateTime | When pricing starts (UTC) |
| EffectiveTo | DateTime? | When pricing ends (NULL = current) |
| RegistrationPrice | decimal | Standard registration price/year |
| RenewalPrice | decimal | Renewal price/year |
| TransferPrice | decimal | Transfer price |
| PrivacyPrice | decimal? | Privacy protection price/year |
| FirstYearRegistrationPrice | decimal? | Special first-year promo price |
| Currency | string | ISO 4217 |
| IsPromotional | bool | Marks temporary promotions |
| PromotionName | string? | Name of promotion |
| Notes | string? | Additional information |
| CreatedBy | string? | User who created this entry |
| CreatedAt | DateTime | Auto-populated |
| UpdatedAt | DateTime | Auto-populated |

**Indexes:**
- `IX_TldSalesPricing_TldId_EffectiveFrom`
- `IX_TldSalesPricing_EffectiveTo`

### **ResellerTldDiscount**
Stores customer-specific discounts (linked to ResellerCompany).

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| ResellerCompanyId | int | FK to ResellerCompany |
| TldId | int | FK to Tld |
| DiscountPercentage | decimal? | Percentage off (e.g., 10.0 = 10%) |
| DiscountAmount | decimal? | Fixed amount off |
| Currency | string? | For DiscountAmount |
| EffectiveFrom | DateTime | When discount starts |
| EffectiveTo | DateTime? | When discount ends |
| ApplyToRegistration | bool | Apply to registrations |
| ApplyToRenewal | bool | Apply to renewals |
| ApplyToTransfer | bool | Apply to transfers |
| IsActive | bool | Currently active |
| Notes | string? | Additional information |
| CreatedBy | string? | User who created this entry |
| CreatedAt | DateTime | Auto-populated |
| UpdatedAt | DateTime | Auto-populated |

**Indexes:**
- `IX_ResellerTldDiscount_ResellerCompanyId_TldId`
- `IX_ResellerTldDiscount_EffectiveTo`

**Business Rule**: Cannot stack with promotional pricing (one or the other).

### **RegistrarSelectionPreference**
Defines how to choose between multiple registrars.

| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| RegistrarId | int | FK to Registrar |
| Priority | int | Selection priority (1 = highest) |
| OffersHosting | bool | Supports hosting bundling |
| OffersEmail | bool | Supports email bundling |
| OffersSsl | bool | Supports SSL bundling |
| MaxCostDifferenceThreshold | decimal? | Max cost diff to prefer this registrar |
| AlwaysPrefer | bool | Override cost logic |
| IsActive | bool | Currently available |
| Notes | string? | Additional information |
| CreatedAt | DateTime | Auto-populated |
| UpdatedAt | DateTime | Auto-populated |

**Indexes:**
- `IX_RegistrarSelectionPreference_RegistrarId`
- `IX_RegistrarSelectionPreference_Priority`

### **RegistrarTld Changes**
Keep for relationships and operational settings:

**KEEP:**
- RegistrarId, TldId
- IsActive, AutoRenew
- MinRegistrationYears, MaxRegistrationYears
- Notes

**DEPRECATE (keep temporarily, remove after migration):**
- RegistrationCost, RegistrationPrice
- RenewalCost, RenewalPrice
- TransferCost, TransferPrice
- PrivacyCost, PrivacyPrice
- Currency

## ?? Migration Strategy

### **Phase 1: Add New Tables** ?
1. Create new entity classes ?
2. Update existing entities with navigation properties ?
3. Update DbContext with new DbSets
4. Create and apply migration
5. Create indexes for performance

### **Phase 2: Data Migration**
1. Migrate existing `RegistrarTld` prices to new tables:
   - Cost prices ? `RegistrarTldCostPricing` (EffectiveFrom = earliest CreatedAt, EffectiveTo = NULL)
   - Sales prices ? `TldSalesPricing` (EffectiveFrom = earliest CreatedAt, EffectiveTo = NULL)
2. Set CreatedBy = "System Migration"
3. Verify all prices migrated correctly
4. Keep original columns for rollback safety

### **Phase 3: Update Service Layer**
1. Create new service methods:
   - `GetCurrentCostPricingAsync(registrarTldId)`
   - `GetCurrentSalesPricingAsync(tldId, currency?)`
   - `GetEffectivePriceAsync(tldId, date, currency?)`
   - `SchedulePriceChangeAsync(...)` 
   - `ApplyResellerDiscountAsync(resellerCompanyId, tldId, ...)`
   - `SelectBestRegistrarAsync(tldId, bundleWithHosting?)`
2. Update existing methods to read from new tables
3. Add temporal query helpers

### **Phase 4: Update Controllers**
1. Add new endpoints for price management:
   - `POST /api/registrar-tlds/{id}/cost-pricing`
   - `GET /api/registrar-tlds/{id}/cost-pricing/history`
   - `POST /api/tlds/{id}/sales-pricing`
   - `GET /api/tlds/{id}/sales-pricing/history`
   - `POST /api/reseller-companies/{id}/tld-discounts`
   - `GET /api/tlds/{id}/effective-price?date=2025-06-01&currency=EUR`
2. Update DTOs to include pricing from new tables
3. Deprecate old price properties in DTOs (mark as obsolete)

### **Phase 5: Testing**
1. Test temporal queries (current, future, historical)
2. Test price calculation with discounts
3. Test registrar selection logic
4. Test scheduled price changes (mock date/time)
5. Test multi-currency scenarios

### **Phase 6: Cleanup (After Verification)**
1. Remove deprecated price columns from `RegistrarTld`
2. Remove old DTOs/endpoints if not needed
3. Update documentation

## ?? Key Query Patterns

### Get Current Cost Price
```csharp
var currentCost = await _context.RegistrarTldCostPricing
    .Where(p => p.RegistrarTldId == registrarTldId &&
                p.EffectiveFrom <= DateTime.UtcNow &&
                (p.EffectiveTo == null || p.EffectiveTo >= DateTime.UtcNow))
    .OrderByDescending(p => p.EffectiveFrom)
    .FirstOrDefaultAsync();
```

### Get Current Sales Price
```csharp
var currentPrice = await _context.TldSalesPricing
    .Where(p => p.TldId == tldId &&
                p.Currency == currency &&
                p.EffectiveFrom <= DateTime.UtcNow &&
                (p.EffectiveTo == null || p.EffectiveTo >= DateTime.UtcNow))
    .OrderByDescending(p => p.EffectiveFrom)
    .FirstOrDefaultAsync();
```

### Get Price at Specific Date
```csharp
var priceAtDate = await _context.TldSalesPricing
    .Where(p => p.TldId == tldId &&
                p.EffectiveFrom <= targetDate &&
                (p.EffectiveTo == null || p.EffectiveTo >= targetDate))
    .OrderByDescending(p => p.EffectiveFrom)
    .FirstOrDefaultAsync();
```

### Apply Reseller Discount
```csharp
var discount = await _context.ResellerTldDiscounts
    .Where(d => d.ResellerCompanyId == resellerCompanyId &&
                d.TldId == tldId &&
                d.IsActive &&
                d.EffectiveFrom <= DateTime.UtcNow &&
                (d.EffectiveTo == null || d.EffectiveTo >= DateTime.UtcNow))
    .FirstOrDefaultAsync();

if (discount != null && discount.ApplyToRegistration)
{
    if (discount.DiscountPercentage.HasValue)
        finalPrice = basePrice * (1 - discount.DiscountPercentage.Value / 100);
    else if (discount.DiscountAmount.HasValue)
        finalPrice = basePrice - discount.DiscountAmount.Value;
}

// Business Rule: If promotional pricing exists, use it OR discount, not both
if (salesPricing.IsPromotional && discount != null)
{
    // Choose better deal for customer (or enforce: no stacking)
    var promoPrice = salesPricing.FirstYearRegistrationPrice ?? salesPricing.RegistrationPrice;
    finalPrice = Math.Min(promoPrice, finalPrice);
}
```

### Select Best Registrar
```csharp
// Get all registrars offering this TLD with current pricing
var registrarsWithPricing = await _context.RegistrarTlds
    .Where(rt => rt.TldId == tldId && rt.IsActive)
    .Include(rt => rt.Registrar)
        .ThenInclude(r => r.SelectionPreferences)
    .Include(rt => rt.CostPricingHistory
        .Where(cp => cp.EffectiveFrom <= DateTime.UtcNow &&
                    (cp.EffectiveTo == null || cp.EffectiveTo >= DateTime.UtcNow)))
    .ToListAsync();

// Find cheapest
var cheapest = registrarsWithPricing
    .OrderBy(rt => rt.CostPricingHistory.FirstOrDefault()?.RegistrationCost ?? decimal.MaxValue)
    .FirstOrDefault();

// If bundling with hosting, prefer hosting registrars within threshold
if (bundleWithHosting)
{
    var hostingRegistrar = registrarsWithPricing
        .Where(rt => rt.Registrar.SelectionPreferences.Any(sp => sp.OffersHosting && sp.IsActive))
        .OrderBy(rt => rt.CostPricingHistory.FirstOrDefault()?.RegistrationCost ?? decimal.MaxValue)
        .FirstOrDefault();
    
    var cheapestCost = cheapest?.CostPricingHistory.FirstOrDefault()?.RegistrationCost ?? 0;
    var hostingCost = hostingRegistrar?.CostPricingHistory.FirstOrDefault()?.RegistrationCost ?? decimal.MaxValue;
    
    var threshold = hostingRegistrar?.Registrar.SelectionPreferences
        .FirstOrDefault()?.MaxCostDifferenceThreshold ?? 0;
    
    if (hostingCost - cheapestCost <= threshold)
        return hostingRegistrar;
}

return cheapest;
```

## ?? Timeline Estimate

| Phase | Duration | Tasks |
|-------|----------|-------|
| Phase 1 | 1 day | Create entities, update DbContext, migration |
| Phase 2 | 1 day | Data migration script + verification |
| Phase 3 | 2-3 days | Service layer updates + helpers |
| Phase 4 | 2-3 days | Controller endpoints + DTOs |
| Phase 5 | 2-3 days | Comprehensive testing |
| Phase 6 | 1 day | Cleanup and documentation |
| **Total** | **9-12 days** | |

## ?? Risks & Mitigations

| Risk | Mitigation |
|------|------------|
| Data loss during migration | Keep original columns, create backups, verify counts |
| Performance degradation | Add proper indexes, use compiled queries, cache current prices |
| Breaking existing API clients | Version API (v1 ? v2), maintain compatibility layer |
| Incorrect price calculations | Extensive unit tests, edge case coverage |
| Timezone issues | Always use UTC for EffectiveFrom/EffectiveTo |

## ?? Best Practices

1. **Always query current prices**: Don't cache for too long (prices change at midnight)
2. **Handle missing prices gracefully**: Log warnings, use fallback logic
3. **Audit all price changes**: Store CreatedBy, log in application logs
4. **Schedule price changes conservatively**: Test in staging first
5. **Multi-currency**: Always specify currency in queries
6. **First-year promotions**: Check FirstYearRegistrationPrice before RegistrationPrice
7. **Discount validation**: Ensure only one discount type applies (promo XOR customer)
8. **Multi-year registrations**: Lock price at purchase time - future price changes don't affect existing registrations
9. **Negative margin alerts**: Monitor and alert when cost > sales price
10. **Future price editing**: Allow editing scheduled prices before EffectiveFrom for flexibility

## ?? Multi-Year Domain Pricing Behavior

**Key Principle**: Price is locked at purchase time for the entire registration period.

### Example Scenario
- Customer purchases `example.com` for 3 years on **Jan 1, 2025**
- Price at purchase: **$12/year** = **$36 total** (paid upfront)
- Registrar cost at purchase: **$8/year** = **$24 total**

**Timeline**:
```
Jan 1, 2025:  Purchase for 3 years @ $12/year
              Customer pays: $36
              Registrar charges: $24
              Margin: $12

Mar 1, 2025:  You increase sales price to $14/year
              ? Does NOT affect existing registration
              ? Customer renewal in 2028 will be at new price

Jan 1, 2026:  Registrar increases cost to $9/year
              ? Does NOT affect existing registration
              ? Your cost is locked at $8/year until renewal

Jan 1, 2028:  Domain expires, customer renews for 3 years
              ? New price applies: $14/year = $42 total
              ? New cost applies: $9/year = $27 total
```

**Database Recording**:
```csharp
// When domain is registered (Jan 1, 2025)
var domain = new RegisteredDomain
{
    Name = "example.com",
    RegistrationDate = new DateTime(2025, 1, 1),
    ExpiryDate = new DateTime(2028, 1, 1),
    RegistrationYears = 3,
    
    // LOCK PRICES AT PURCHASE TIME
    PricePerYear = 12.00m,  // From TldSalesPricing on Jan 1, 2025
    TotalPrice = 36.00m,    // 3 years × $12
    
    CostPerYear = 8.00m,    // From RegistrarTldCostPricing on Jan 1, 2025
    TotalCost = 24.00m,     // 3 years × $8
    
    Currency = "USD"
};
```

**Why This Works**:
- ? Customer and registrar both paid upfront for full period
- ? No recalculation needed during the registration period
- ? Historical pricing preserved for this transaction
- ? Future price changes only affect new purchases/renewals

**Important Notes**:
1. **Auto-renewal**: If enabled, renewal will use pricing at renewal date (not original)
2. **Transfer pricing**: Transfer-in uses current pricing, not original registration
3. **Invoicing**: Invoice shows historical prices from purchase date
4. **Margin reporting**: Use prices from purchase date for accurate historical margins

## ?? Next Steps

1. Review this migration plan
2. Decide on Phase 1 execution date
3. Create database backup strategy
4. Plan API versioning approach
5. Notify API clients of upcoming changes
6. Create rollback plan

## ? Resolved Design Decisions

- [x] **Approval workflow**: Not required - scheduled prices auto-activate at EffectiveFrom
- [x] **Edit scheduled prices**: YES - allowed before EffectiveFrom passes
- [x] **Multi-year domains**: No issue - paid in advance, price locked at purchase time
- [x] **Archive policy**: Configurable via appsettings (default: 7 years cost, 5 years sales)
- [x] **Negative margins**: Alert when registrar cost > sales price
- [x] **Currency conversion**: Automatic using exchange rate service
- [x] **Price notifications**: Send alerts when margins are negative/low
