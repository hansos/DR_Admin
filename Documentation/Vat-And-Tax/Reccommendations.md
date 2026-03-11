# VAT and TAX Recommendations

## Scope

Applies to:

- `DR_Admin` API
- `DR_Admin_ResellerPanel`
- `DR_Admin_UserPanel`

## Recommended Approach

### 1) Centralize tax logic in `DR_Admin` API

Use the API as the single source of truth for VAT/TAX:

- `TaxDeterminationService`
- `TaxCalculationService`
- `InvoiceTaxService`

Panels should only send inputs and display API-calculated outputs.

### 2) Required tax decision inputs

For each transaction, evaluate and store:

- Seller legal entity and registrations
- Buyer type (`B2C` / `B2B`)
- Buyer location evidence (billing country, IP, VAT ID)
- Product/service tax category
- Transaction date (rate-effective date)
- Currency and FX timestamp

### 3) International tax handling

Support jurisdiction-specific rules:

- EU VAT destination rules for digital services
- OSS/IOSS where applicable
- B2B reverse charge with VAT ID validation
- US sales tax (nexus-driven, non-VAT)
- Export/zero-rate/exempt scenarios

Rules should be versioned and date-effective.

### 4) Multi-currency strategy

- Calculate tax in the jurisdiction/base tax currency first
- Convert for customer display with a locked FX rate at quote/checkout
- Persist:
  - Net amount
  - Tax amount
  - Gross amount
  - Tax currency
  - Display currency
  - FX rate, source, and timestamp

Do not recalculate historical invoices using new FX rates.

### 5) Precision and rounding

- Use `decimal` for all monetary calculations
- Define one consistent rounding strategy (line-level or invoice-level)
- Respect currency minor units (e.g., JPY 0, KWD 3)

### 6) Data model recommendations

At minimum:

- `TaxJurisdiction`
- `TaxRate` (effective from/to, region, category)
- `TaxRegistration`
- `CustomerTaxProfile`
- `OrderTaxSnapshot` (frozen input/output for audit)

Snapshotting is required for invoices, refunds, and compliance.

### 7) Panel responsibilities

Panels should provide:

- Billing address/country
- VAT ID (if available)
- Selected currency
- Product lines/categories

API should return:

- Line tax breakdown
- Total tax and gross totals
- Reverse-charge flags
- Legal labels/notes for invoice display

No panel-side tax formulas.

### 8) Compliance and audit trail

Store and retain:

- VAT ID validation evidence
- Location evidence used for tax decision
- Applied rule version
- Invoice tax breakdown
- Refund/credit-note tax adjustments

## Rollout Plan

1. Add tax entities + snapshot tables in `DR_Admin`.
2. Implement deterministic calculator with test coverage (EU/US/B2B/B2C).
3. Expose API endpoints:
   - `POST /tax/quote`
   - `POST /tax/finalize`
4. Update both panels to consume only API tax outputs.
5. Ensure invoice templates include jurisdiction/rate breakdown.
6. Add admin tax/rule management or integrate a provider.
