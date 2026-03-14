# CreateTaxRuleDto

Data transfer object for creating a tax rule

## Source

`DR_Admin/DTOs/CreateTaxRuleDto.cs`

## TypeScript Interface

```ts
export interface CreateTaxRuleDto {
  taxCategoryId: number | null;
  countryCode: string;
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
  internalNotes: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `TaxCategoryId` | `int?` | `number | null` |
| `CountryCode` | `string` | `string` |
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
| `InternalNotes` | `string` | `string` |

## Used By Endpoints

- [POST CreateTaxRule](../tax-rules/post-create-tax-rule-api-v1-tax-rules.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

