# SyncResultDto

Data transfer object for SyncResultDto.

## Source

`DR_Admin/DTOs/HostingAccountDto.cs`

## TypeScript Interface

```ts
export interface SyncResultDto {
  success: boolean;
  message: string;
  recordsSynced: number;
  syncedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Success` | `bool` | `boolean` |
| `Message` | `string` | `string` |
| `RecordsSynced` | `int` | `number` |
| `SyncedAt` | `DateTime` | `string` |

## Used By Endpoints

- [POST ProvisionAccountOnCPanel](../hosting-accounts/post-provision-account-on-c-panel-api-v1-hosting-accounts-id-provision-on-cpanel.md)
- [POST SyncDatabasesFromServer](../hosting-databases/post-sync-databases-from-server-api-v1-hosting-accounts-hostingaccountid-databases-sync.md)
- [POST SyncDomainsFromServer](../hosting-domains/post-sync-domains-from-server-api-v1-hosting-accounts-hostingaccountid-domains-sync.md)
- [POST SyncEmailAccountsFromServer](../hosting-email/post-sync-email-accounts-from-server-api-v1-hosting-accounts-hostingaccountid-emails-sync.md)
- [POST SyncFtpAccountsFromServer](../hosting-ftp/post-sync-ftp-accounts-from-server-api-v1-hosting-accounts-hostingaccountid-ftp-sync.md)
- [POST ExportAccountToServer](../hosting-sync/post-export-account-to-server-api-v1-hosting-sync-export-hostingaccountid.md)
- [POST ImportAccountFromServer](../hosting-sync/post-import-account-from-server-api-v1-hosting-sync-import.md)
- [POST ImportAllAccountsFromServer](../hosting-sync/post-import-all-accounts-from-server-api-v1-hosting-sync-import-all-servercontrolpanelid.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

