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

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
