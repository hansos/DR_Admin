# TldSalesPricingDto

DTO for displaying TLD sales pricing information

## Source

`DR_Admin/DTOs/TldPricingDtos.cs`

## TypeScript Interface

```ts
export interface TldSalesPricingDto {
  id: number;
  tldId: number;
  tldExtension: string | null;
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
  createdBy: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `TldId` | `int` | `number` |
| `TldExtension` | `string?` | `string | null` |
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
| `CreatedBy` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetCurrentSalesPricing](../tld-pricing/get-get-current-sales-pricing-api-v1-tld-pricing-sales-tld-tldid-current.md)
- [POST CreateSalesPricing](../tld-pricing/post-create-sales-pricing-api-v1-tld-pricing-sales.md)
- [PUT UpdateSalesPricing](../tld-pricing/put-update-sales-pricing-api-v1-tld-pricing-sales-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

