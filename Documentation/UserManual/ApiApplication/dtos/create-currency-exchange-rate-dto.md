# CreateCurrencyExchangeRateDto

Data transfer object for creating a currency exchange rate

## Source

`DR_Admin/DTOs/CreateCurrencyExchangeRateDto.cs`

## TypeScript Interface

```ts
export interface CreateCurrencyExchangeRateDto {
  baseCurrency: string;
  targetCurrency: string;
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
| `BaseCurrency` | `string` | `string` |
| `TargetCurrency` | `string` | `string` |
| `Rate` | `decimal` | `number` |
| `EffectiveDate` | `DateTime` | `string` |
| `ExpiryDate` | `DateTime?` | `string | null` |
| `Source` | `CurrencyRateSource` | `CurrencyRateSource` |
| `IsActive` | `bool` | `boolean` |
| `Markup` | `decimal` | `number` |
| `Notes` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
