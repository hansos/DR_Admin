# UpdateTaxRuleDto

Data transfer object for updating a tax rule

## Source

`DR_Admin/DTOs/UpdateTaxRuleDto.cs`

## TypeScript Interface

```ts
export interface UpdateTaxRuleDto {
  taxCategoryId: number | null;
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

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
