# OperatingSystemDto

Data transfer object representing an operating system

## Source

`DR_Admin/DTOs/OperatingSystemDto.cs`

## TypeScript Interface

```ts
export interface OperatingSystemDto {
  id: number;
  name: string;
  displayName: string;
  description: string | null;
  version: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Name` | `string` | `string` |
| `DisplayName` | `string` | `string` |
| `Description` | `string?` | `string | null` |
| `Version` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
