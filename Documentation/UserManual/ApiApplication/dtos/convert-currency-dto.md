# ConvertCurrencyDto

Data transfer object for currency conversion request

## Source

`DR_Admin/DTOs/ConvertCurrencyDto.cs`

## TypeScript Interface

```ts
export interface ConvertCurrencyDto {
  amount: number;
  fromCurrency: string;
  toCurrency: string;
  rateDate: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Amount` | `decimal` | `number` |
| `FromCurrency` | `string` | `string` |
| `ToCurrency` | `string` | `string` |
| `RateDate` | `DateTime?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
