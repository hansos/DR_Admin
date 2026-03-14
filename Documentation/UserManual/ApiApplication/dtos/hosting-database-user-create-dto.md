# HostingDatabaseUserCreateDto

Data transfer object for HostingDatabaseUserCreateDto.

## Source

`DR_Admin/DTOs/HostingResourceDto.cs`

## TypeScript Interface

```ts
export interface HostingDatabaseUserCreateDto {
  hostingDatabaseId: number;
  username: string;
  password: string;
  privileges: string[] | null;
  allowedHosts: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `HostingDatabaseId` | `int` | `number` |
| `Username` | `string` | `string` |
| `Password` | `string` | `string` |
| `Privileges` | `List<string>?` | `string[] | null` |
| `AllowedHosts` | `string?` | `string | null` |

## Used By Endpoints

- [POST CreateDatabaseUser](../hosting-databases/post-create-database-user-api-v1-hosting-accounts-hostingaccountid-databases-databaseid-users.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

