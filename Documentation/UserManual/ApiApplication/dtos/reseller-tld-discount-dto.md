# ResellerTldDiscountDto

DTO for displaying reseller TLD discount information

## Source

`DR_Admin/DTOs/TldPricingDtos.cs`

## TypeScript Interface

```ts
export interface ResellerTldDiscountDto {
  id: number;
  resellerCompanyId: number;
  resellerCompanyName: string | null;
  tldId: number;
  tldExtension: string | null;
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
  createdBy: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `ResellerCompanyId` | `int` | `number` |
| `ResellerCompanyName` | `string?` | `string | null` |
| `TldId` | `int` | `number` |
| `TldExtension` | `string?` | `string | null` |
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
| `CreatedBy` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

No endpoint pages currently reference this DTO.

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

