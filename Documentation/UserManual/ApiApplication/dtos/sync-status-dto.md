# SyncStatusDto

Data transfer object for SyncStatusDto.

## Source

`DR_Admin/DTOs/HostingAccountDto.cs`

## TypeScript Interface

```ts
export interface SyncStatusDto {
  hostingAccountId: number;
  syncStatus: string;
  lastSyncedAt: string | null;
  externalAccountId: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `HostingAccountId` | `int` | `number` |
| `SyncStatus` | `string` | `string` |
| `LastSyncedAt` | `DateTime?` | `string | null` |
| `ExternalAccountId` | `string?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
