# CurrencyExchangeRateDto

Data transfer object for currency exchange rate

## Source

`DR_Admin/DTOs/CurrencyExchangeRateDto.cs`

## TypeScript Interface

```ts
export interface CurrencyExchangeRateDto {
  id: number;
  baseCurrency: string;
  targetCurrency: string;
  rate: number;
  effectiveDate: string;
  expiryDate: string | null;
  source: CurrencyRateSource;
  isActive: boolean;
  markup: number;
  effectiveRate: number;
  notes: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `BaseCurrency` | `string` | `string` |
| `TargetCurrency` | `string` | `string` |
| `Rate` | `decimal` | `number` |
| `EffectiveDate` | `DateTime` | `string` |
| `ExpiryDate` | `DateTime?` | `string | null` |
| `Source` | `CurrencyRateSource` | `CurrencyRateSource` |
| `IsActive` | `bool` | `boolean` |
| `Markup` | `decimal` | `number` |
| `EffectiveRate` | `decimal` | `number` |
| `Notes` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
