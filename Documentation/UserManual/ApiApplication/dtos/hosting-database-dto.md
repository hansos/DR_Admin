# HostingDatabaseDto

Data transfer object for HostingDatabaseDto.

## Source

`DR_Admin/DTOs/HostingResourceDto.cs`

## TypeScript Interface

```ts
export interface HostingDatabaseDto {
  id: number;
  hostingAccountId: number;
  databaseName: string;
  databaseType: string;
  sizeMB: number | null;
  serverHost: string | null;
  serverPort: number | null;
  externalDatabaseId: string | null;
  lastSyncedAt: string | null;
  syncStatus: string | null;
  databaseUsers: HostingDatabaseUserDto[] | null;
  createdAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `HostingAccountId` | `int` | `number` |
| `DatabaseName` | `string` | `string` |
| `DatabaseType` | `string` | `string` |
| `SizeMB` | `int?` | `number | null` |
| `ServerHost` | `string?` | `string | null` |
| `ServerPort` | `int?` | `number | null` |
| `ExternalDatabaseId` | `string?` | `string | null` |
| `LastSyncedAt` | `DateTime?` | `string | null` |
| `SyncStatus` | `string?` | `string | null` |
| `DatabaseUsers` | `List<HostingDatabaseUserDto>?` | `HostingDatabaseUserDto[] | null` |
| `CreatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetDatabase](../hosting-databases/get-get-database-api-v1-hosting-accounts-hostingaccountid-databases-id.md)
- [POST CreateDatabase](../hosting-databases/post-create-database-api-v1-hosting-accounts-hostingaccountid-databases.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

