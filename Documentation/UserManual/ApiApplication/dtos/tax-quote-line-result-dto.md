# TaxQuoteLineResultDto

Represents tax calculation result for a single line.

## Source

`DR_Admin/DTOs/TaxQuoteLineResultDto.cs`

## TypeScript Interface

```ts
export interface TaxQuoteLineResultDto {
  lineId: number | null;
  description: string;
  netAmount: number;
  taxRate: number;
  taxAmount: number;
  grossAmount: number;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `LineId` | `int?` | `number | null` |
| `Description` | `string` | `string` |
| `NetAmount` | `decimal` | `number` |
| `TaxRate` | `decimal` | `number` |
| `TaxAmount` | `decimal` | `number` |
| `GrossAmount` | `decimal` | `number` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
