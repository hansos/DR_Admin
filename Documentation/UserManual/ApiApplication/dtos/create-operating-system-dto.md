# CreateOperatingSystemDto

Data transfer object for creating a new operating system

## Source

`DR_Admin/DTOs/OperatingSystemDto.cs`

## TypeScript Interface

```ts
export interface CreateOperatingSystemDto {
  name: string;
  displayName: string;
  description: string | null;
  version: string | null;
  isActive: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `DisplayName` | `string` | `string` |
| `Description` | `string?` | `string | null` |
| `Version` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
