# TaxRuleDto

Data transfer object representing a tax rule

## Source

`DR_Admin/DTOs/TaxRuleDto.cs`

## TypeScript Interface

```ts
export interface TaxRuleDto {
  id: number;
  countryCode: string;
  taxCategoryId: number | null;
  stateCode: string | null;
  taxName: string;
  taxCategory: string;
  taxRate: number;
  isActive: boolean;
  effectiveFrom: string;
  effectiveUntil: string | null;
  appliesToSetupFees: boolean;
  appliesToRecurring: boolean;
  reverseCharge: boolean;
  taxAuthority: string;
  taxRegistrationNumber: string;
  priority: number;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CountryCode` | `string` | `string` |
| `TaxCategoryId` | `int?` | `number | null` |
| `StateCode` | `string?` | `string | null` |
| `TaxName` | `string` | `string` |
| `TaxCategory` | `string` | `string` |
| `TaxRate` | `decimal` | `number` |
| `IsActive` | `bool` | `boolean` |
| `EffectiveFrom` | `DateTime` | `string` |
| `EffectiveUntil` | `DateTime?` | `string | null` |
| `AppliesToSetupFees` | `bool` | `boolean` |
| `AppliesToRecurring` | `bool` | `boolean` |
| `ReverseCharge` | `bool` | `boolean` |
| `TaxAuthority` | `string` | `string` |
| `TaxRegistrationNumber` | `string` | `string` |
| `Priority` | `int` | `number` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
