# RegistrarTldCostPricingDto

DTO for displaying registrar TLD cost pricing information

## Source

`DR_Admin/DTOs/TldPricingDtos.cs`

## TypeScript Interface

```ts
export interface RegistrarTldCostPricingDto {
  id: number;
  registrarTldId: number;
  registrarName: string | null;
  tldExtension: string | null;
  effectiveFrom: string;
  effectiveTo: string | null;
  registrationCost: number;
  renewalCost: number;
  transferCost: number;
  privacyCost: number | null;
  firstYearRegistrationCost: number | null;
  currency: string;
  isActive: boolean;
  notes: string | null;
  createdBy: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `RegistrarTldId` | `int` | `number` |
| `RegistrarName` | `string?` | `string | null` |
| `TldExtension` | `string?` | `string | null` |
| `EffectiveFrom` | `DateTime` | `string` |
| `EffectiveTo` | `DateTime?` | `string | null` |
| `RegistrationCost` | `decimal` | `number` |
| `RenewalCost` | `decimal` | `number` |
| `TransferCost` | `decimal` | `number` |
| `PrivacyCost` | `decimal?` | `number | null` |
| `FirstYearRegistrationCost` | `decimal?` | `number | null` |
| `Currency` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |
| `CreatedBy` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetCurrentCostPricing](../registrar-tld-cost-pricing/get-get-current-cost-pricing-api-v1-registrar-tld-cost-pricing-registrar-tld-registrartldid-current.md)
- [POST CreateCostPricing](../registrar-tld-cost-pricing/post-create-cost-pricing-api-v1-registrar-tld-cost-pricing.md)
- [PUT UpdateCostPricing](../registrar-tld-cost-pricing/put-update-cost-pricing-api-v1-registrar-tld-cost-pricing-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

