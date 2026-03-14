# CreateRegistrarTldCostPricingDto

DTO for creating new registrar TLD cost pricing

## Source

`DR_Admin/DTOs/TldPricingDtos.cs`

## TypeScript Interface

```ts
export interface CreateRegistrarTldCostPricingDto {
  registrarTldId: number;
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
| `RegistrarTldId` | `int` | `number` |
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

- [POST CreateCostPricing](../registrar-tld-cost-pricing/post-create-cost-pricing-api-v1-registrar-tld-cost-pricing.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

