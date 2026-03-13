# CalculatePricingResponse

Response with calculated pricing

## Source

`DR_Admin/DTOs/TldPricingDtos.cs`

## TypeScript Interface

```ts
export interface CalculatePricingResponse {
  tldExtension: string | null;
  basePrice: number;
  discountAmount: number;
  finalPrice: number;
  currency: string;
  isPromotionalPricing: boolean;
  promotionName: string | null;
  isDiscountApplied: boolean;
  discountDescription: string | null;
  selectedRegistrarId: number | null;
  selectedRegistrarName: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `TldExtension` | `string?` | `string | null` |
| `BasePrice` | `decimal` | `number` |
| `DiscountAmount` | `decimal` | `number` |
| `FinalPrice` | `decimal` | `number` |
| `Currency` | `string` | `string` |
| `IsPromotionalPricing` | `bool` | `boolean` |
| `PromotionName` | `string?` | `string | null` |
| `IsDiscountApplied` | `bool` | `boolean` |
| `DiscountDescription` | `string?` | `string | null` |
| `SelectedRegistrarId` | `int?` | `number | null` |
| `SelectedRegistrarName` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
