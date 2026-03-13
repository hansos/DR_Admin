# CurrencyDto

Data transfer object representing a supported currency.

## Source

`DR_Admin/DTOs/CurrencyDto.cs`

## TypeScript Interface

```ts
export interface CurrencyDto {
  id: number;
  code: string;
  name: string;
  symbol: string | null;
  isActive: boolean;
  isDefault: boolean;
  isCustomerCurrency: boolean;
  isProviderCurrency: boolean;
  sortOrder: number;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Code` | `string` | `string` |
| `Name` | `string` | `string` |
| `Symbol` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `IsDefault` | `bool` | `boolean` |
| `IsCustomerCurrency` | `bool` | `boolean` |
| `IsProviderCurrency` | `bool` | `boolean` |
| `SortOrder` | `int` | `number` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
