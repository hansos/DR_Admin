# SystemSettingDto

Data transfer object representing a system setting

## Source

`DR_Admin/DTOs/SystemSettingDto.cs`

## TypeScript Interface

```ts
export interface SystemSettingDto {
  id: number;
  key: string;
  value: string;
  description: string;
  isSystemKey: boolean;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Key` | `string` | `string` |
| `Value` | `string` | `string` |
| `Description` | `string` | `string` |
| `IsSystemKey` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetSystemSettingById](../system-settings/get-get-system-setting-by-id-api-v1-system-settings-id-int.md)
- [GET GetSystemSettingByKey](../system-settings/get-get-system-setting-by-key-api-v1-system-settings-key-key.md)
- [POST CreateSystemSetting](../system-settings/post-create-system-setting-api-v1-system-settings.md)
- [PUT UpdateSystemSetting](../system-settings/put-update-system-setting-api-v1-system-settings-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

