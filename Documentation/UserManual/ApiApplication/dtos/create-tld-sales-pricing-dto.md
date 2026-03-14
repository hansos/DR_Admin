# CreateTldSalesPricingDto

DTO for creating new TLD sales pricing

## Source

`DR_Admin/DTOs/TldPricingDtos.cs`

## TypeScript Interface

```ts
export interface CreateTldSalesPricingDto {
  tldId: number;
  effectiveFrom: string;
  effectiveTo: string | null;
  registrationPrice: number;
  renewalPrice: number;
  transferPrice: number;
  privacyPrice: number | null;
  firstYearRegistrationPrice: number | null;
  currency: string;
  isPromotional: boolean;
  promotionName: string | null;
  isActive: boolean;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `TldId` | `int` | `number` |
| `EffectiveFrom` | `DateTime` | `string` |
| `EffectiveTo` | `DateTime?` | `string | null` |
| `RegistrationPrice` | `decimal` | `number` |
| `RenewalPrice` | `decimal` | `number` |
| `TransferPrice` | `decimal` | `number` |
| `PrivacyPrice` | `decimal?` | `number | null` |
| `FirstYearRegistrationPrice` | `decimal?` | `number | null` |
| `Currency` | `string` | `string` |
| `IsPromotional` | `bool` | `boolean` |
| `PromotionName` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |

## Used By Endpoints

- [POST CreateSalesPricing](../tld-pricing/post-create-sales-pricing-api-v1-tld-pricing-sales.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

