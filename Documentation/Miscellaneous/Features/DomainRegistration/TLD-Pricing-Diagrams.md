# TLD Pricing Architecture - Entity Relationship Diagram

```
┌──────────────────────┐
│   ResellerCompany    │
│──────────────────────│
│ Id                   │
│ Name                 │
│ DefaultCurrency      │
│ ...                  │
└──────────────────────┘
         │
         │ 1:N
         ▼
┌──────────────────────────────┐
│   ResellerTldDiscount        │
│──────────────────────────────│
│ Id                           │
│ ResellerCompanyId (FK)       │
│ TldId (FK) ────────────────┐ │
│ DiscountPercentage         │ │
│ DiscountAmount             │ │
│ EffectiveFrom  ◄───────────┼─┼─── Temporal
│ EffectiveTo                │ │
│ ApplyToRegistration        │ │
│ ApplyToRenewal             │ │
│ ApplyToTransfer            │ │
│ IsActive                   │ │
└──────────────────────────────┘ 
                             │
                             │
       ┌─────────────────────┘
       │
       │
       ▼
┌──────────────────────┐             ┌──────────────────────────────┐
│        Tld           │             │    TldSalesPricing           │
│──────────────────────│             │──────────────────────────────│
│ Id                   │◄──────────┐ │ Id                           │
│ Extension            │        1:N│ │ TldId (FK)                  │
│ Description          │           │ │ EffectiveFrom  ◄────────────┼─── Temporal
│ IsActive             │           │ │ EffectiveTo                 │
│ ...                  │           │ │ RegistrationPrice           │
└──────────────────────┘           │ │ RenewalPrice                │
         │                         │ │ TransferPrice               │
         │ 1:N                     │ │ FirstYearRegistrationPrice  │
         ▼                         │ │ IsPromotional               │
┌──────────────────────┐           │ │ PromotionName               │
│   RegistrarTld       │           │ │ Currency                    │
│──────────────────────│           │ └─────────────────────────────┘
│ Id                   │           │
│ RegistrarId (FK) ────┼────┐      │
│ TldId (FK)           │    │      └── One sales price per TLD
│ IsActive             │    │          (all registrars share same customer price)
│ MinRegistrationYears │    │
│ MaxRegistrationYears │    │
│ AutoRenew            │    │
│ Notes                │    │
└──────────────────────┘    │
         │                  │
         │ 1:N              │
         ▼                  └──────┐
┌──────────────────────────────┐   │
│ RegistrarTldCostPricing      │   │
│──────────────────────────────│   │
│ Id                           │   │
│ RegistrarTldId (FK)          │   │
│ EffectiveFrom  ◄─────────────┼───┼─── Temporal
│ EffectiveTo                  │   │
│ RegistrationCost             │   │
│ RenewalCost                  │   │
│ TransferCost                 │   │
│ FirstYearRegistrationCost    │   │
│ Currency (varies by registrar)   │
└──────────────────────────────┘   │
                                   │
                                   │
       ┌───────────────────────────┘
       │
       ▼
┌──────────────────────┐             ┌────────────────────────────────┐
│     Registrar        │             │ RegistrarSelectionPreference   │
│──────────────────────│             │────────────────────────────────│
│ Id                   │◄──────────┐ │ Id                             │
│ Name                 │        1:N│ │ RegistrarId (FK)               │
│ Code                 │           │ │ Priority (1=highest)           │
│ IsActive             │           │ │ OffersHosting                  │
│ IsDefault            │           │ │ OffersEmail                    │
│ ...                  │           │ │ OffersSsl                      │
└──────────────────────┘           │ │ MaxCostDifferenceThreshold     │
                                   │ │ AlwaysPrefer                   │
                                   │ │ IsActive                       │
                                   │ └────────────────────────────────┘
                                   │
                                   └── Selection logic for choosing
                                       between multiple registrars
```

## Data Flow: Customer Purchase

```
Customer wants to buy "example.com"
         │
         ▼
    ┌────────────────────────────────────┐
    │  1. Get TLD Sales Pricing          │
    │  Query: TldSalesPricing            │
    │  WHERE TldId = .com                │
    │    AND EffectiveFrom <= NOW        │
    │    AND (EffectiveTo IS NULL        │
    │         OR EffectiveTo >= NOW)     │
    └────────────────────────────────────┘
         │
         ▼ Base Price = $12.00 (or $7.99 if FirstYear promo)
    ┌────────────────────────────────────┐
    │  2. Check Customer Discount        │
    │  Query: ResellerTldDiscount        │
    │  WHERE ResellerCompanyId = 123     │
    │    AND TldId = .com                │
    │    AND IsActive = true             │
    │    AND EffectiveFrom <= NOW        │
    │    AND (EffectiveTo IS NULL        │
    │         OR EffectiveTo >= NOW)     │
    └────────────────────────────────────┘
         │
         ▼ Discount = 10% (or promo if better)
    ┌────────────────────────────────────┐
    │  3. Calculate Final Price          │
    │  Business Rule:                    │
    │  - Promo OR Discount (not both)    │
    │  - Choose better deal for customer │
    └────────────────────────────────────┘
         │
         ▼ Customer pays $7.99 (promo wins)
    ┌────────────────────────────────────┐
    │  4. Select Best Registrar          │
    │  Query: RegistrarTld + Pricing     │
    │  WHERE TldId = .com                │
    │    AND IsActive = true             │
    │  JOIN: Current CostPricing         │
    │  ORDER BY: RegistrationCost ASC    │
    └────────────────────────────────────┘
         │
         ▼ Registrar B: $7.50 cost (offers hosting)
    ┌────────────────────────────────────┐
    │  5. Check Bundling Preference      │
    │  Query: RegistrarSelectionPref     │
    │  WHERE RegistrarId = B             │
    │    AND OffersHosting = true        │
    │    AND IsActive = true             │
    └────────────────────────────────────┘
         │
         ▼ Customer has hosting: prefer Registrar B
    ┌────────────────────────────────────┐
    │  6. Complete Purchase              │
    │  Customer pays: $7.99              │
    │  Registrar cost: $7.50             │
    │  Margin: $0.49                     │
    └────────────────────────────────────┘
```

## Temporal Pricing Timeline

```
Timeline: .com Sales Pricing History

2024-01-01 ──────────────────────┐
                                  │ Price: $12.00
2024-11-25 ──────────────────────┤ EffectiveTo: 2024-11-24
                                  │
2024-11-25 ──────────────────────┐ Price: $7.99 (Promo)
                                  │ IsPromotional: true
2024-11-30 ──────────────────────┤ PromotionName: "Black Friday"
                                  │
2024-12-01 ──────────────────────┐ Price: $12.00
                                  │ EffectiveTo: 2025-02-28
2025-03-01 ──────────────────────┤
                                  │
2025-03-01 ──────────────────────┐ Price: $14.00
                                  │ EffectiveTo: NULL (current)
    NOW ──────────────────────────┤
                                  │
Future queries use:               │
- EffectiveFrom <= TargetDate     │
- EffectiveTo IS NULL OR          │
  EffectiveTo >= TargetDate       │
```

## Currency Flow (Multi-Currency Example)

```
Registrar A (GoDaddy)          Registrar B (Namecheap)
  Currency: USD                    Currency: EUR
  Cost: $8.50                      Cost: €7.80
         │                                │
         └────────────┬───────────────────┘
                      │
                      ▼
              Convert to common currency
              for comparison (if needed)
                      │
                      ▼
         Select cheapest in base currency
                      │
                      ▼
            Store cost in original currency
         (RegistrarTldCostPricing.Currency)
                      │
                      ▼
         TldSalesPricing.Currency = "USD"
         (Customer always sees USD price)
```

## Margin Analysis Query

```
SELECT 
    t.Extension,
    r.Name AS RegistrarName,
    cost.RegistrationCost,
    cost.Currency AS CostCurrency,
    sales.RegistrationPrice,
    sales.Currency AS SalesCurrency,
    sales.RegistrationPrice - cost.RegistrationCost AS Margin,
    ((sales.RegistrationPrice - cost.RegistrationCost) / sales.RegistrationPrice * 100) AS MarginPct
FROM Tld t
JOIN RegistrarTld rt ON rt.TldId = t.Id AND rt.IsActive = true
JOIN Registrar r ON r.Id = rt.RegistrarId
LEFT JOIN RegistrarTldCostPricing cost ON cost.RegistrarTldId = rt.Id
    AND cost.EffectiveFrom <= GETUTCDATE()
    AND (cost.EffectiveTo IS NULL OR cost.EffectiveTo >= GETUTCDATE())
LEFT JOIN TldSalesPricing sales ON sales.TldId = t.Id
    AND sales.EffectiveFrom <= GETUTCDATE()
    AND (sales.EffectiveTo IS NULL OR sales.EffectiveTo >= GETUTCDATE())
WHERE cost.Currency = sales.Currency  -- Only compare same currency
ORDER BY Margin DESC;
```

## Example Scenarios

### Scenario 1: Schedule Price Increase (3 months ahead)

**Today: 2024-12-01**
```sql
-- Current pricing
SELECT * FROM TldSalesPricing WHERE TldId = 1 AND EffectiveTo IS NULL;
-- Returns: RegistrationPrice = $12.00, EffectiveFrom = 2024-01-01

-- Schedule increase for March 1, 2025
INSERT INTO TldSalesPricing (TldId, EffectiveFrom, EffectiveTo, RegistrationPrice, ...)
VALUES (1, '2025-03-01', NULL, 14.00, ...);

UPDATE TldSalesPricing 
SET EffectiveTo = '2025-02-28' 
WHERE TldId = 1 AND EffectiveTo IS NULL AND EffectiveFrom < '2025-03-01';

-- Result:
-- 2024-12-01 to 2025-02-28: $12.00
-- 2025-03-01 onwards: $14.00 (automatic)
```

### Scenario 2: VIP Customer Gets 15% Off

```sql
INSERT INTO ResellerTldDiscount (
    ResellerCompanyId, TldId, 
    DiscountPercentage, 
    EffectiveFrom, EffectiveTo,
    ApplyToRegistration, ApplyToRenewal, ApplyToTransfer,
    IsActive
)
VALUES (
    123, 1,           -- ResellerCompany 123, TLD .com
    15.0,             -- 15% discount
    '2025-01-01', '2025-12-31',
    true, true, true,
    true
);

-- When customer 123 buys .com:
-- Base price: $12.00
-- Discount: $12.00 * 0.15 = $1.80
-- Final price: $12.00 - $1.80 = $10.20
```

### Scenario 3: Registrar Updates Their Costs

```sql
-- GoDaddy increases .com cost from $8.50 to $9.00 effective 2025-02-01

-- Close current pricing
UPDATE RegistrarTldCostPricing 
SET EffectiveTo = '2025-01-31' 
WHERE RegistrarTldId = 5 AND EffectiveTo IS NULL;

-- Create new pricing
INSERT INTO RegistrarTldCostPricing (
    RegistrarTldId, EffectiveFrom, EffectiveTo,
    RegistrationCost, RenewalCost, TransferCost, Currency
)
VALUES (
    5, '2025-02-01', NULL,
    9.00, 9.00, 9.00, 'USD'
);

-- Margin analysis will now show reduced margin after Feb 1
-- May trigger alert if margin drops below threshold
```

---

## Key Takeaways

1. **Temporal Pattern**: EffectiveFrom/EffectiveTo enables historical tracking and future scheduling
2. **Separation**: Cost pricing (registrar) vs Sales pricing (customer) vs Discounts (reseller)
3. **One Price Per TLD**: Customers see consistent pricing regardless of registrar
4. **Smart Selection**: Choose cheapest registrar, but consider bundling opportunities
5. **Multi-Currency**: Each registrar can charge in different currency
6. **Business Rules**: No stacking discounts, promotional pricing takes precedence
7. **Audit Trail**: CreatedBy, CreatedAt, UpdatedAt track all changes

---

Ready to implement? Start with Phase 1: Database Migration! 🚀
