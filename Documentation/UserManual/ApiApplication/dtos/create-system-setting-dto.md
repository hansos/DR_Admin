# CreateSystemSettingDto

Data transfer object for creating a new system setting

## Source

`DR_Admin/DTOs/SystemSettingDto.cs`

## TypeScript Interface

```ts
export interface CreateSystemSettingDto {
  key: string;
  value: string;
  description: string;
  isSystemKey: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Key` | `string` | `string` |
| `Value` | `string` | `string` |
| `Description` | `string` | `string` |
| `IsSystemKey` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
