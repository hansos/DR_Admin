# UpdateSystemSettingDto

Data transfer object for updating an existing system setting

## Source

`DR_Admin/DTOs/SystemSettingDto.cs`

## TypeScript Interface

```ts
export interface UpdateSystemSettingDto {
  value: string;
  description: string;
  isSystemKey: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Value` | `string` | `string` |
| `Description` | `string` | `string` |
| `IsSystemKey` | `bool` | `boolean` |

## Used By Endpoints

- [PUT UpdateSystemSetting](../system-settings/put-update-system-setting-api-v1-system-settings-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

