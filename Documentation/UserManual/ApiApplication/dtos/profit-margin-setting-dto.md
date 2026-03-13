# ProfitMarginSettingDto

Data transfer object for profit margin settings.

## Source

`DR_Admin/DTOs/ProfitMarginSettingDtos.cs`

## TypeScript Interface

```ts
export interface ProfitMarginSettingDto {
  id: number;
  productClass: ProfitProductClass;
  profitPercent: number;
  isActive: boolean;
  notes: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `ProductClass` | `ProfitProductClass` | `ProfitProductClass` |
| `ProfitPercent` | `decimal` | `number` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
