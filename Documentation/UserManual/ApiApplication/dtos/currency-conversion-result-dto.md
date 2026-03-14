# CurrencyConversionResultDto

Data transfer object for currency conversion result

## Source

`DR_Admin/DTOs/CurrencyConversionResultDto.cs`

## TypeScript Interface

```ts
export interface CurrencyConversionResultDto {
  originalAmount: number;
  fromCurrency: string;
  toCurrency: string;
  exchangeRate: number;
  convertedAmount: number;
  rateDate: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `OriginalAmount` | `decimal` | `number` |
| `FromCurrency` | `string` | `string` |
| `ToCurrency` | `string` | `string` |
| `ExchangeRate` | `decimal` | `number` |
| `ConvertedAmount` | `decimal` | `number` |
| `RateDate` | `DateTime` | `string` |

## Used By Endpoints

- [POST ConvertCurrency](../currencies/post-convert-currency-api-v1-currencies-convert.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

