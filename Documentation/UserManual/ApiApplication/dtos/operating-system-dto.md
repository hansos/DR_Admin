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

## Used By Endpoints

- [GET GetOperatingSystemById](../operating-systems/get-get-operating-system-by-id-api-v1-operating-systems-id.md)
- [POST CreateOperatingSystem](../operating-systems/post-create-operating-system-api-v1-operating-systems.md)
- [PUT UpdateOperatingSystem](../operating-systems/put-update-operating-system-api-v1-operating-systems-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

