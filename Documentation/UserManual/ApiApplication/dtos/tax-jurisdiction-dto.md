# TaxJurisdictionDto

Data transfer object representing a tax jurisdiction.

## Source

`DR_Admin/DTOs/TaxJurisdictionDto.cs`

## TypeScript Interface

```ts
export interface TaxJurisdictionDto {
  id: number;
  code: string;
  name: string;
  countryCode: string;
  stateCode: string | null;
  taxAuthority: string;
  taxCurrencyCode: string;
  isActive: boolean;
  notes: string;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Code` | `string` | `string` |
| `Name` | `string` | `string` |
| `CountryCode` | `string` | `string` |
| `StateCode` | `string?` | `string | null` |
| `TaxAuthority` | `string` | `string` |
| `TaxCurrencyCode` | `string` | `string` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string` | `string` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
