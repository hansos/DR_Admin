# UpdateResellerTldDiscountDto

DTO for updating existing reseller TLD discount

## Source

`DR_Admin/DTOs/TldPricingDtos.cs`

## TypeScript Interface

```ts
export interface UpdateResellerTldDiscountDto {
  effectiveFrom: string;
  effectiveTo: string | null;
  discountPercentage: number | null;
  discountAmount: number | null;
  discountCurrency: string | null;
  applyToRegistration: boolean;
  applyToRenewal: boolean;
  applyToTransfer: boolean;
  isActive: boolean;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `EffectiveFrom` | `DateTime` | `string` |
| `EffectiveTo` | `DateTime?` | `string | null` |
| `DiscountPercentage` | `decimal?` | `number | null` |
| `DiscountAmount` | `decimal?` | `number | null` |
| `DiscountCurrency` | `string?` | `string | null` |
| `ApplyToRegistration` | `bool` | `boolean` |
| `ApplyToRenewal` | `bool` | `boolean` |
| `ApplyToTransfer` | `bool` | `boolean` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

