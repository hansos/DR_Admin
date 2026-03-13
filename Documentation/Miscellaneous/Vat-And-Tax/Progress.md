# VAT and TAX Progress

## Completed

### 1) Recommendation document

- Added `Documentation/Vat-And-Tax/Reccommendations.md` with the recommended VAT/TAX architecture and rollout plan.

### 2) API entity foundation

- Added entities:
  - `TaxJurisdiction`
  - `TaxRegistration`
  - `OrderTaxSnapshot`
- Updated entities:
  - `TaxRule` (added optional `TaxJurisdictionId` relation)
  - `Order` (added `TaxSnapshots` navigation)
- Updated `ApplicationDbContext`:
  - Added `DbSet`s for new entities
  - Added Fluent API configurations (indexes, precision, relationships)

### 3) Migration

- Created migration:
  - `20260311102827_AddVatTaxFoundationEntities`
- Migration includes new tables and `TaxRules.TaxJurisdictionId` column with FK.

### 4) DTOs

- Added DTOs for CRUD flows:
  - `TaxJurisdictionDto`, `CreateTaxJurisdictionDto`, `UpdateTaxJurisdictionDto`
  - `TaxRegistrationDto`, `CreateTaxRegistrationDto`, `UpdateTaxRegistrationDto`
  - `OrderTaxSnapshotDto`, `CreateOrderTaxSnapshotDto`, `UpdateOrderTaxSnapshotDto`

### 5) Services (CRUD)

- Added service interfaces:
  - `ITaxJurisdictionService`
  - `ITaxRegistrationService`
  - `IOrderTaxSnapshotService`
- Added implementations:
  - `TaxJurisdictionService`
  - `TaxRegistrationService`
  - `OrderTaxSnapshotService`

### 6) Controllers (CRUD)

- Added controllers:
  - `TaxJurisdictionsController`
  - `TaxRegistrationsController`
  - `OrderTaxSnapshotsController`
- Implemented full CRUD endpoints.
- Used policy-based authorization on endpoints (`[Authorize(Policy = "...")]`).

### 7) Authorization policies

- Added policy sets in `AuthorizationPoliciesConfiguration`:
  - `TaxJurisdiction.Read/Write/Delete`
  - `TaxRegistration.Read/Write/Delete`
  - `OrderTaxSnapshot.Read/Write/Delete`

### 8) DI registration

- Registered services in `Program.cs`:
  - `ITaxJurisdictionService -> TaxJurisdictionService`
  - `ITaxRegistrationService -> TaxRegistrationService`
  - `IOrderTaxSnapshotService -> OrderTaxSnapshotService`

### 9) Validation

- Built `DR_Admin/DR_Admin.csproj` successfully after changes.
- Existing warnings remain unrelated to this VAT/TAX work.

### 10) Tax quote/finalize workflow

- Added tax calculation API endpoints:
  - `POST /api/v1/tax/quote`
  - `POST /api/v1/tax/finalize`
- Added DTOs for request/response and line-level breakdown:
  - `TaxQuoteRequestDto`
  - `TaxQuoteResultDto`
  - `TaxQuoteLineRequestDto`
  - `TaxQuoteLineResultDto`
- Added calculation service:
  - `ITaxCalculationService`
  - `TaxCalculationService`

### 11) VAT validation abstraction

- Added VAT validation interface and default implementation:
  - `IVatValidationService`
  - `VatValidationService`
- Wired validation into tax calculation and tax service flows.

### 12) Tax service implementation upgrade

- Replaced stub logic in `TaxService` with concrete implementation for:
  - Tax rule CRUD operations
  - Active/effective-date tax rule filtering
  - Location/priority-based rule selection
  - Reverse-charge behavior for B2B + validated tax ID
  - VAT validation integration

### 13) Idempotent tax finalization

- Extended `OrderTaxSnapshot` with `IdempotencyKey`.
- Added unique index for `(OrderId, IdempotencyKey)` in DbContext configuration.
- Updated order tax snapshot DTOs and service mapping to include idempotency key.
- Added migration:
  - `20260311105706_AddOrderTaxSnapshotIdempotency`

### 14) Policy enforcement updates

- Updated `TaxRulesController` endpoints to use policy-based authorization consistently.
- Added new authorization policies:
  - `TaxCalculation.Quote`
  - `TaxCalculation.Finalize`

### 15) Additional DI registrations

- Registered in `Program.cs`:
  - `IVatValidationService -> VatValidationService`
  - `ITaxCalculationService -> TaxCalculationService`

### 16) Tax category normalization and country/state scoping

- Added `TaxCategory` table entity scoped by:
  - `CountryCode`
  - optional `StateCode`
  - `Code` (e.g., `STANDARD`, `REDUCED`, `EXEMPT`)
- Added unique index on `(CountryCode, StateCode, Code)`.
- Linked `TaxRule` to `TaxCategory` via optional `TaxCategoryId` foreign key.
- Kept `TaxRule.TaxCategory` code field for backward compatibility and rule matching.

### 17) Tax category API surface

- Added DTOs:
  - `TaxCategoryDto`
  - `CreateTaxCategoryDto`
  - `UpdateTaxCategoryDto`
- Added service:
  - `ITaxCategoryService`
  - `TaxCategoryService`
- Added controller:
  - `TaxCategoriesController`
- Added policies:
  - `TaxCategory.Read`
  - `TaxCategory.Write`
  - `TaxCategory.Delete`
- Registered in DI:
  - `ITaxCategoryService -> TaxCategoryService`

### 18) Tax rule/category consistency rules

- `TaxService` now supports `TaxCategoryId` in tax rule create/update DTOs.
- When `TaxCategoryId` is provided, tax rule country/state/category are resolved from the selected `TaxCategory` row.
- Added overlap prevention for tax rules by:
  - country
  - state
  - tax category
  - effective date range

### 19) Tax calculation hardening

- `FinalizeTaxAsync` now requires:
  - valid `OrderId`
  - non-empty `IdempotencyKey`
- Finalize validates order existence and customer ownership consistency.
- Tax calculation now resolves rules per line using `TaxQuoteLineRequestDto.TaxCategory`.
- Supports mixed tax results across lines and persists applied rule IDs in snapshot input JSON.

### 20) Build validation status

- Validation retried after fixing plugin provider project references.
- Workspace build now succeeds.

### 21) Invoice tax-source enforcement from finalized snapshots

- Extended `Invoice` with optional links:
  - `OrderId`
  - `OrderTaxSnapshotId`
- Updated invoice DTOs (`InvoiceDto`, `CreateInvoiceDto`, `UpdateInvoiceDto`) with those references.
- Updated `ApplicationDbContext` invoice mapping:
  - indexes for `OrderId` and `OrderTaxSnapshotId`
  - FKs to `Orders` and `OrderTaxSnapshots`
- Updated `InvoiceService.CreateInvoiceAsync`:
  - validates `OrderId` and customer ownership when order-linked
  - auto-loads latest tax snapshot for order when `OrderTaxSnapshotId` is not provided
  - enforces snapshot presence for order-linked invoices
  - overrides invoice tax fields (`SubTotal`, `TaxAmount`, `TotalAmount`, `TaxRate`, `TaxName`) from finalized snapshot
- Added migration:
  - `20260311124000_EnforceInvoiceTaxSnapshotSource`

### 22) Current validation caveat

- Full build may fail while API is running due locked output DLLs during debug sessions.
- Modified files are free of direct diagnostics in editor checks.

### 23) Invoice update enforcement for snapshot-linked tax

- Updated `InvoiceService.UpdateInvoiceAsync` to enforce finalized snapshot tax source when:
  - `OrderId` is provided, or
  - `OrderTaxSnapshotId` is provided.
- Added validation in update flow:
  - verifies `OrderId` exists
  - verifies invoice customer matches order customer
  - verifies referenced snapshot exists
  - verifies snapshot belongs to provided order
  - requires latest finalized snapshot for order-linked updates when snapshot ID is omitted
- For snapshot-linked updates, tax fields are now sourced from snapshot:
  - `SubTotal` (from `NetAmount`)
  - `TaxAmount`
  - `TotalAmount` (from `GrossAmount`)
  - `TaxRate`
  - `TaxName`
- This prevents manual tax drift on invoices tied to finalized order tax snapshots.

### 24) VAT validation provider architecture

- Refactored VAT validation into provider-based pipeline:
  - `IVatValidationProvider`
  - `BuiltInVatValidationProvider`
  - `VatValidationService` now orchestrates providers
- Added detailed validation output model:
  - `VatValidationResult` (validity, provider name, raw response)
- Extended `IVatValidationService` with:
  - `ValidateDetailedAsync(...)`
- Registered provider in DI:
  - `IVatValidationProvider -> BuiltInVatValidationProvider`

### 25) Tax determination evidence model (compliance)

- Added normalized evidence entity:
  - `TaxDeterminationEvidence`
- Captures key evidence fields:
  - buyer/billing country
  - buyer state
  - source IP
  - buyer tax ID and validation outcome
  - validation provider/raw response
  - exchange-rate source
  - captured timestamp
- Added `DbSet<TaxDeterminationEvidence>` and Fluent config in `ApplicationDbContext`.
- Linked `OrderTaxSnapshot` to evidence via:
  - `TaxDeterminationEvidenceId` (optional FK)

### 26) Trusted FX governance in tax flow

- Added request controls in `TaxQuoteRequestDto`:
  - `RequireTrustedExchangeRate`
  - `BillingCountryCode`
  - `IpAddress`
- Added FX provenance fields in `TaxQuoteResultDto`:
  - `ExchangeRate`
  - `ExchangeRateDate`
  - `ExchangeRateSource`
  - `TaxDeterminationEvidenceId`
- `TaxCalculationService` now resolves exchange rates from `CurrencyExchangeRates` when currencies differ.
- If trusted FX is required (or finalize path), calculation fails when no trusted rate exists.
- Snapshot persistence now stores:
  - resolved exchange rate/date/source
  - linked evidence record ID

### 27) Snapshot DTO/service propagation

- Extended snapshot DTOs with:
  - `ExchangeRateSource`
  - `TaxDeterminationEvidenceId`
- Updated `OrderTaxSnapshotService` mapping for those fields.

### 28) Migration added for evidence + trusted FX fields

- Added migration:
  - `20260311133000_AddTaxEvidenceAndTrustedFx`
- Migration includes:
  - new `TaxDeterminationEvidences` table
  - new `OrderTaxSnapshots.ExchangeRateSource`
  - new `OrderTaxSnapshots.TaxDeterminationEvidenceId`
  - FK/indexes for evidence linkage

### 29) Snapshot immutability enforcement

- Enforced production immutability for `OrderTaxSnapshot` records in `OrderTaxSnapshotService`:
  - update operations now throw an immutable-state exception
  - delete operations now throw an immutable-state exception
  - explicit protection when snapshot is referenced by an invoice
- Updated `OrderTaxSnapshotsController` to return `409 Conflict` for immutable-state violations.

### 30) Tax API error behavior hardening

- Updated `TaxCalculationController` (`quote` and `finalize`) to return `400 BadRequest` for domain/business validation failures (`InvalidOperationException`) instead of generic failures.

### 31) Evidence minimum requirements on finalize

- Added strict minimum evidence checks for finalize path in `TaxCalculationService`:
  - `BillingCountryCode` is now required
  - `IpAddress` is now required and validated as a real IP address
- This strengthens audit and compliance readiness for finalized tax snapshots.

### 32) Integration tests for production hardening paths

- Added integration test suite:
  - `DR_Admin.IntegrationTests/Controllers/TaxProductionHardeningTests.cs`
- Covered scenarios:
  - `FinalizeTax` returns `400 BadRequest` when required evidence (`BillingCountryCode`) is missing.
  - `FinalizeTax` valid flow persists both:
    - `OrderTaxSnapshot`
    - `TaxDeterminationEvidence`
  - `OrderTaxSnapshots` update/delete endpoints return `409 Conflict` due enforced snapshot immutability.
- Build validated successfully after adding tests.

### 33) Additional integration tests for FX trust + idempotency

- Extended `TaxProductionHardeningTests` with scenarios for:
  - trusted FX required + no available trusted rate -> `400 BadRequest` on finalize
  - trusted FX required + seeded trusted rate -> finalize succeeds with persisted FX source/rate
  - repeated finalize with same `IdempotencyKey` -> returns same `SnapshotId`
- Added exchange-rate seeding helper in tests for deterministic FX behavior validation.
- Build validated successfully after extending test coverage.

### 34) Stripe VAT provider implementation

- Implemented `StripeVatValidationProvider` as an `IVatValidationProvider`.
- Provider behavior:
  - uses configured Stripe secret key
  - creates a temporary Stripe customer
  - submits tax ID via Stripe customer tax ID endpoint
  - reads Stripe verification status
  - cleans up temporary customer
  - maps Stripe verification result into `VatValidationResult`
- Added country-to-Stripe tax type mapping support:
  - `eu_vat` for EU countries
  - `gb_vat`, `au_abn`, `nz_gst` for supported non-EU paths
- Updated DI registration in `Program.cs`:
  - added typed HTTP client for Stripe provider
  - registered `StripeVatValidationProvider`
  - retained `BuiltInVatValidationProvider` as fallback
- Improved `VatValidationService` diagnostics by returning last provider response when all providers fail.
- Build validated successfully after Stripe provider integration.
