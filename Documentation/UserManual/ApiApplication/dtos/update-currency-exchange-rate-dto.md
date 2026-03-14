# UpdateCurrencyExchangeRateDto

Data transfer object for updating a currency exchange rate

## Source

`DR_Admin/DTOs/UpdateCurrencyExchangeRateDto.cs`

## TypeScript Interface

```ts
export interface UpdateCurrencyExchangeRateDto {
  rate: number;
  effectiveDate: string;
  expiryDate: string | null;
  source: CurrencyRateSource;
  isActive: boolean;
  markup: number;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Rate` | `decimal` | `number` |
| `EffectiveDate` | `DateTime` | `string` |
| `ExpiryDate` | `DateTime?` | `string | null` |
| `Source` | `CurrencyRateSource` | `CurrencyRateSource` |
| `IsActive` | `bool` | `boolean` |
| `Markup` | `decimal` | `number` |
| `Notes` | `string?` | `string | null` |

## Used By Endpoints

- [PUT UpdateRate](../currencies/put-update-rate-api-v1-currencies-rates-id-int.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

