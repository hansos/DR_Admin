# HostingDatabaseUserDto

Data transfer object for HostingDatabaseUserDto.

## Source

`DR_Admin/DTOs/HostingResourceDto.cs`

## TypeScript Interface

```ts
export interface HostingDatabaseUserDto {
  id: number;
  hostingDatabaseId: number;
  username: string;
  privileges: string | null;
  allowedHosts: string | null;
  externalUserId: string | null;
  lastSyncedAt: string | null;
  syncStatus: string | null;
  createdAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `HostingDatabaseId` | `int` | `number` |
| `Username` | `string` | `string` |
| `Privileges` | `string?` | `string | null` |
| `AllowedHosts` | `string?` | `string | null` |
| `ExternalUserId` | `string?` | `string | null` |
| `LastSyncedAt` | `DateTime?` | `string | null` |
| `SyncStatus` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [POST CreateDatabaseUser](../hosting-databases/post-create-database-user-api-v1-hosting-accounts-hostingaccountid-databases-databaseid-users.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

