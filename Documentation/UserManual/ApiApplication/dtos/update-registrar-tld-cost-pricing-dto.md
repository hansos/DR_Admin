# UpdateRegistrarTldCostPricingDto

DTO for updating existing registrar TLD cost pricing

## Source

`DR_Admin/DTOs/TldPricingDtos.cs`

## TypeScript Interface

```ts
export interface UpdateRegistrarTldCostPricingDto {
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
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
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

## Used By Endpoints

- [PUT UpdateCostPricing](../registrar-tld-cost-pricing/put-update-cost-pricing-api-v1-registrar-tld-cost-pricing-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

