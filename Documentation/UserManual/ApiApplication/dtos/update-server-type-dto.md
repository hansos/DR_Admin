# UpdateServerTypeDto

Data transfer object for updating an existing server type

## Source

`DR_Admin/DTOs/ServerTypeDto.cs`

## TypeScript Interface

```ts
export interface UpdateServerTypeDto {
  name: string;
  displayName: string;
  description: string | null;
  isActive: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `DisplayName` | `string` | `string` |
| `Description` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |

## Used By Endpoints

- [PUT UpdateServerType](../server-types/put-update-server-type-api-v1-server-types-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

