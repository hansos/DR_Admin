# TaxQuoteLineRequestDto

Represents an input line for tax quote or finalize calculation.

## Source

`DR_Admin/DTOs/TaxQuoteLineRequestDto.cs`

## TypeScript Interface

```ts
export interface TaxQuoteLineRequestDto {
  lineId: number | null;
  description: string;
  taxCategory: string;
  netAmount: number;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `LineId` | `int?` | `number | null` |
| `Description` | `string` | `string` |
| `TaxCategory` | `string` | `string` |
| `NetAmount` | `decimal` | `number` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

