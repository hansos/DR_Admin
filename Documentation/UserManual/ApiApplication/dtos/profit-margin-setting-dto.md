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

## Used By Endpoints

- [GET GetById](../profit-margin-settings/get-get-by-id-api-v1-profit-margin-settings-id-int.md)
- [GET GetByProductClass](../profit-margin-settings/get-get-by-product-class-api-v1-profit-margin-settings-by-class-productclass.md)
- [POST Create](../profit-margin-settings/post-create-api-v1-profit-margin-settings.md)
- [PUT Update](../profit-margin-settings/put-update-api-v1-profit-margin-settings-id-int.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

