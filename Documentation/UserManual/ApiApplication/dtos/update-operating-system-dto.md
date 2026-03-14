# UpdateOperatingSystemDto

Data transfer object for updating an existing operating system

## Source

`DR_Admin/DTOs/OperatingSystemDto.cs`

## TypeScript Interface

```ts
export interface UpdateOperatingSystemDto {
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

## Used By Endpoints

- [PUT UpdateOperatingSystem](../operating-systems/put-update-operating-system-api-v1-operating-systems-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

