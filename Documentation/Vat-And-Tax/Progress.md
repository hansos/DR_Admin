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
