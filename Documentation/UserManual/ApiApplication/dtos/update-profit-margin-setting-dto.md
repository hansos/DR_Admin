# UpdateProfitMarginSettingDto

Data transfer object for updating a profit margin setting.

## Source

`DR_Admin/DTOs/ProfitMarginSettingDtos.cs`

## TypeScript Interface

```ts
export interface UpdateProfitMarginSettingDto {
  profitPercent: number;
  isActive: boolean;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `ProfitPercent` | `decimal` | `number` |
| `IsActive` | `bool` | `boolean` |
| `Notes` | `string?` | `string | null` |

## Used By Endpoints

- [PUT Update](../profit-margin-settings/put-update-api-v1-profit-margin-settings-id-int.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

