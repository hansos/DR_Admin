# ?? TLD Pricing Architecture - Summary

## What We've Created

Based on your requirements, I've designed a comprehensive temporal pricing system that separates:
1. **Registrar costs** (what they charge you)
2. **Customer sales prices** (what you charge customers)  
3. **Customer discounts** (limited discounts for ResellerCompany)
4. **Registrar selection logic** (choose cheapest, prefer bundling)

---

## ? New Files Created

### **Entity Classes**
- `RegistrarTldCostPricing.cs` - Tracks registrar costs over time
- `TldSalesPricing.cs` - Tracks your sales prices over time
- `ResellerTldDiscount.cs` - Customer-specific discounts
- `RegistrarSelectionPreference.cs` - Registrar selection rules

### **Service Interfaces**
- `ITldPricingService.cs` - Complete pricing service contract

### **DTOs**
- `TldPricingDtos.cs` - All DTOs for pricing endpoints

### **Documentation**
- `TLD-Pricing-Migration-Guide.md` - Complete migration plan

### **Updated Entities**
- `RegistrarTld.cs` - Added `CostPricingHistory` navigation
- `Tld.cs` - Added `SalesPricingHistory` and `ResellerDiscounts` navigation
- `Registrar.cs` - Added `SelectionPreferences` navigation
- `ResellerCompany.cs` - Added `TldDiscounts` navigation
- `ApplicationDbContext.cs` - Added new DbSets

---

## ?? Key Design Decisions

### 1. **Separation of Concerns**
? **Registrar costs** stored separately from **sales prices**
- Allows different currencies per registrar
- Complete audit trail for both cost and revenue
- Enables margin analysis

### 2. **Temporal Pricing**
? Uses `EffectiveFrom` / `EffectiveTo` pattern
- Auto-activation when EffectiveFrom date passes
- Historical data preservation (7 years costs, 5 years sales)
- Schedule future price changes

### 3. **One Price Per TLD**
? `TldSalesPricing` linked to `TldId` (NOT `RegistrarTldId`)
- Customers see one price regardless of which registrar fulfills
- You choose cheapest registrar behind the scenes
- Simplifies customer experience

### 4. **Limited Discounts**
? `ResellerTldDiscount` linked to `ResellerCompany`
- Percentage OR fixed amount discount
- Cannot stack with promotional pricing (business rule)
- Temporal (can expire)
- Operation-specific (registration, renewal, transfer)

### 5. **First-Year Promotions**
? `FirstYearRegistrationCost/Price` fields
- Registrars often give discounts on first year
- Your promotions can also target first year
- Separate from regular multi-year pricing

### 6. **Smart Registrar Selection**
? `RegistrarSelectionPreference` table
- Primary: Choose cheapest registrar
- Secondary: Prefer bundling (hosting, email, SSL)
- Configurable cost threshold for bundling preference
- Priority-based fallback

### 7. **Multi-Currency Support**
? Each registrar can have different currency
- `RegistrarTldCostPricing.Currency` varies by registrar
- `TldSalesPricing.Currency` for your sales prices
- No forced USD conversion

---

## ?? Data Flow Examples

### **Example 1: Customer Buys .com Domain**

1. **Customer requests**: `.com` for 1 year
2. **System queries**: Current `TldSalesPricing` for .com
   - Finds: $12.00 (or $7.99 if first year promotion active)
3. **Check discount**: Query `ResellerTldDiscount` for this customer
   - Finds: 10% discount
   - Apply: $12.00 * 0.9 = $10.80
4. **Business rule**: Promotional price ($7.99) vs Discount ($10.80)
   - Choose: $7.99 (better for customer, or enforce "no stacking")
5. **Select registrar**: Query all registrars offering .com
   - Registrar A: $8.00 cost
   - Registrar B: $7.50 cost (also offers hosting)
   - If customer has hosting: Choose B (bundling preference)
   - Otherwise: Choose B (cheapest)
6. **Margin**: $7.99 (revenue) - $7.50 (cost) = $0.49 profit

### **Example 2: Schedule Price Increase**

**Current**: .com = $12.00 (effective 2024-01-01, EffectiveTo = NULL)

**Schedule for March 1, 2025**:
```csharp
// Create new pricing
INSERT TldSalesPricing:
  TldId = 1 (.com)
  EffectiveFrom = 2025-03-01
  EffectiveTo = NULL
  RegistrationPrice = $14.00
  CreatedBy = "admin@example.com"

// Close current pricing
UPDATE TldSalesPricing WHERE TldId = 1 AND EffectiveTo IS NULL:
  EffectiveTo = 2025-02-28
```

**Result**:
- Feb 28: Customers pay $12.00
- Mar 1: Customers pay $14.00 (automatic)
- Historical data preserved for invoices/auditing

### **Example 3: Black Friday Promotion**

```csharp
INSERT TldSalesPricing:
  TldId = 1 (.com)
  EffectiveFrom = 2025-11-25 00:00:00 UTC
  EffectiveTo = 2025-11-30 23:59:59 UTC
  FirstYearRegistrationPrice = $4.99
  RegistrationPrice = $12.00
  IsPromotional = TRUE
  PromotionName = "Black Friday 2025"
```

**Result**:
- Nov 25-30: First-year .com = $4.99
- Multi-year still $12/year
- Dec 1: Reverts to regular pricing (query finds next EffectiveFrom)

---

## ?? Next Steps to Implement

### **Phase 1: Database Migration** (Day 1)
```bash
# Create migration
dotnet ef migrations add AddTldPricingTables

# Review migration file
# Apply to development database
dotnet ef database update

# Verify tables created
```

### **Phase 2: Data Migration** (Day 1-2)
```sql
-- Migrate existing RegistrarTld costs
INSERT INTO RegistrarTldCostPricing 
  (RegistrarTldId, EffectiveFrom, EffectiveTo, 
   RegistrationCost, RenewalCost, TransferCost, PrivacyCost, Currency, CreatedBy, CreatedAt, UpdatedAt)
SELECT 
  Id, 
  CreatedAt, 
  NULL,
  RegistrationCost, 
  RenewalCost, 
  TransferCost, 
  PrivacyCost, 
  Currency, 
  'System Migration',
  GETUTCDATE(),
  GETUTCDATE()
FROM RegistrarTld;

-- Migrate existing sales prices (one per TLD, take first registrar's price)
INSERT INTO TldSalesPricing 
  (TldId, EffectiveFrom, EffectiveTo,
   RegistrationPrice, RenewalPrice, TransferPrice, PrivacyPrice, Currency, CreatedBy, CreatedAt, UpdatedAt)
SELECT DISTINCT ON (TldId)
  TldId,
  MIN(CreatedAt),
  NULL,
  RegistrationPrice,
  RenewalPrice,
  TransferPrice,
  PrivacyPrice,
  Currency,
  'System Migration',
  GETUTCDATE(),
  GETUTCDATE()
FROM RegistrarTld
GROUP BY TldId, RegistrationPrice, RenewalPrice, TransferPrice, PrivacyPrice, Currency;

-- Verify counts match
SELECT COUNT(*) FROM RegistrarTld;
SELECT COUNT(*) FROM RegistrarTldCostPricing;
SELECT COUNT(DISTINCT TldId) FROM RegistrarTld;
SELECT COUNT(*) FROM TldSalesPricing;
```

### **Phase 3: Service Implementation** (Day 2-4)
1. Implement `TldPricingService`
2. Add temporal query helpers
3. Add price calculation logic
4. Add registrar selection logic
5. Unit tests

### **Phase 4: Controller Updates** (Day 4-6)
1. Create new pricing endpoints
2. Update existing endpoints to use new service
3. Add DTOs for requests/responses
4. API documentation
5. Integration tests

### **Phase 5: Testing** (Day 7-9)
1. Test temporal queries (past, current, future)
2. Test discount calculations
3. Test promotional pricing
4. Test registrar selection
5. Test multi-currency
6. Load testing

### **Phase 6: Cleanup** (Day 10-12)
1. Remove deprecated price columns from `RegistrarTld`
2. Update documentation
3. Deploy to production
4. Monitor for issues

---

## ?? Business Logic Examples

### **Discount Business Rules**

```csharp
// Rule: Cannot stack promotional price with customer discount
if (salesPricing.IsPromotional && resellerDiscount != null)
{
    // Option 1: Give customer the better deal
    var promoPrice = salesPricing.FirstYearRegistrationPrice ?? salesPricing.RegistrationPrice;
    var discountedPrice = basePrice * (1 - resellerDiscount.DiscountPercentage / 100);
    finalPrice = Math.Min(promoPrice, discountedPrice);
    
    // Option 2: Enforce "no stacking" - promotional price only
    finalPrice = promoPrice;
    
    // Option 3: Enforce "no stacking" - customer discount takes precedence
    finalPrice = discountedPrice;
}
```

### **Registrar Selection Logic**

```csharp
// Get all active registrars with current pricing
var candidates = registrarTlds
    .Where(rt => rt.IsActive && rt.CostPricingHistory.Any(cp => cp.EffectiveTo == null))
    .OrderBy(rt => rt.CostPricingHistory.First().RegistrationCost)
    .ToList();

var cheapest = candidates.First();

// If customer wants bundling, prefer registrar offering both
if (bundleWithHosting)
{
    var hostingRegistrar = candidates
        .FirstOrDefault(rt => rt.Registrar.SelectionPreferences
            .Any(sp => sp.OffersHosting && sp.IsActive));
    
    if (hostingRegistrar != null)
    {
        var costDiff = hostingRegistrar.CostPricingHistory.First().RegistrationCost 
                     - cheapest.CostPricingHistory.First().RegistrationCost;
        
        var threshold = hostingRegistrar.Registrar.SelectionPreferences
            .First().MaxCostDifferenceThreshold ?? 0;
        
        if (costDiff <= threshold)
            return hostingRegistrar; // Worth bundling
    }
}

return cheapest;
```

---

## ?? Important Considerations

### **1. Timezone Handling**
- ? Always use UTC for `EffectiveFrom` / `EffectiveTo`
- ? Convert to local timezone only for display
- ? Scheduled price changes trigger at midnight UTC

### **2. Query Performance**
- ? Add indexes on temporal columns
- ? Cache current prices (invalidate daily)
- ? Use compiled queries for frequent lookups

### **3. Data Consistency**
- ? Use transactions when closing old pricing and creating new
- ? Ensure no overlapping EffectiveFrom/EffectiveTo ranges
- ? Validate EffectiveFrom < EffectiveTo

### **4. API Versioning**
- ? Keep old endpoints working (backward compatibility)
- ? Mark deprecated endpoints with `[Obsolete]`
- ? Give clients 6 months to migrate

### **5. Testing Edge Cases**
- ?? What if no current pricing exists?
- ?? What if two prices have same EffectiveFrom?
- ?? What if customer has expired discount?
- ?? What if registrar cost > sales price (negative margin)?

---

## ?? Questions to Resolve

Before starting implementation:

1. **Approval Workflow**: Should price changes require approval before going live?
2. **Edit Future Prices**: Can scheduled prices be edited before EffectiveFrom?
3. **Multi-Year Domains**: How to handle price changes for existing 3-year registrations?
4. **Archive Policy**: When to delete old pricing records (7 years? 10 years? never)?
5. **Notifications**: Email admins when scheduled prices activate?
6. **Negative Margins**: Alert if cost > sales price?
7. **Currency Conversion**: Auto-convert between currencies or manual entry?

---

## ?? Documentation References

- **Migration Guide**: `Documentation/TLD-Pricing-Migration-Guide.md`
- **Entity Models**: `DR_Admin/Data/Entities/`
- **Service Interface**: `DR_Admin/Services/ITldPricingService.cs`
- **DTOs**: `DR_Admin/DTOs/TldPricingDtos.cs`

---

## ? Summary

This architecture gives you:

? **Historical tracking** - Complete audit trail  
? **Future scheduling** - Set-and-forget price changes  
? **Separation of costs/sales** - Clear margin visibility  
? **Customer discounts** - Flexible, controlled discounts  
? **Promotional pricing** - First-year specials, Black Friday, etc.  
? **Multi-currency** - Different registrar currencies  
? **Smart registrar selection** - Cheapest with bundling logic  
? **Scalability** - Temporal queries, indexed properly  

**Estimated Implementation**: 9-12 days  
**Database Impact**: +4 tables, ~5-10 columns removed from RegistrarTld  
**API Impact**: New endpoints, backward-compatible changes  

Ready to start Phase 1? ??
