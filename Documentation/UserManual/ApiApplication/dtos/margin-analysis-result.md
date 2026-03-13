# MarginAnalysisResult

Result of margin analysis calculation

## Source

`DR_Admin/DTOs/TldPricingDtos.cs`

## TypeScript Interface

```ts
export interface MarginAnalysisResult {
  tldId: number;
  tldExtension: string | null;
  registrarId: number | null;
  registrarName: string | null;
  operationType: string;
  cost: number;
  price: number;
  marginAmount: number;
  marginPercentage: number;
  costCurrency: string;
  priceCurrency: string;
  isNegativeMargin: boolean;
  isLowMargin: boolean;
  alertMessage: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `TldId` | `int` | `number` |
| `TldExtension` | `string?` | `string | null` |
| `RegistrarId` | `int?` | `number | null` |
| `RegistrarName` | `string?` | `string | null` |
| `OperationType` | `string` | `string` |
| `Cost` | `decimal` | `number` |
| `Price` | `decimal` | `number` |
| `MarginAmount` | `decimal` | `number` |
| `MarginPercentage` | `decimal` | `number` |
| `CostCurrency` | `string` | `string` |
| `PriceCurrency` | `string` | `string` |
| `IsNegativeMargin` | `bool` | `boolean` |
| `IsLowMargin` | `bool` | `boolean` |
| `AlertMessage` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
