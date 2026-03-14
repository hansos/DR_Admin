# CalculatePricingRequest

Request for calculating pricing with discounts

## Source

`DR_Admin/DTOs/TldPricingDtos.cs`

## TypeScript Interface

```ts
export interface CalculatePricingRequest {
  tldId: number;
  resellerCompanyId: number | null;
  operationType: string;
  years: number;
  isFirstYear: boolean;
  targetCurrency: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `TldId` | `int` | `number` |
| `ResellerCompanyId` | `int?` | `number | null` |
| `OperationType` | `string` | `string` |
| `Years` | `int` | `number` |
| `IsFirstYear` | `bool` | `boolean` |
| `TargetCurrency` | `string?` | `string | null` |

## Used By Endpoints

- [POST CalculatePricing](../tld-pricing/post-calculate-pricing-api-v1-tld-pricing-calculate.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

