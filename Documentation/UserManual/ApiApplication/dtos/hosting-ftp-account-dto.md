# HostingFtpAccountDto

Data transfer object for HostingFtpAccountDto.

## Source

`DR_Admin/DTOs/HostingResourceDto.cs`

## TypeScript Interface

```ts
export interface HostingFtpAccountDto {
  id: number;
  hostingAccountId: number;
  username: string;
  homeDirectory: string;
  quotaMB: number | null;
  readOnly: boolean;
  sftpEnabled: boolean;
  ftpsEnabled: boolean;
  externalFtpId: string | null;
  lastSyncedAt: string | null;
  syncStatus: string | null;
  createdAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `HostingAccountId` | `int` | `number` |
| `Username` | `string` | `string` |
| `HomeDirectory` | `string` | `string` |
| `QuotaMB` | `int?` | `number | null` |
| `ReadOnly` | `bool` | `boolean` |
| `SftpEnabled` | `bool` | `boolean` |
| `FtpsEnabled` | `bool` | `boolean` |
| `ExternalFtpId` | `string?` | `string | null` |
| `LastSyncedAt` | `DateTime?` | `string | null` |
| `SyncStatus` | `string?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetFtpAccount](../hosting-ftp/get-get-ftp-account-api-v1-hosting-accounts-hostingaccountid-ftp-id.md)
- [POST CreateFtpAccount](../hosting-ftp/post-create-ftp-account-api-v1-hosting-accounts-hostingaccountid-ftp.md)
- [PUT UpdateFtpAccount](../hosting-ftp/put-update-ftp-account-api-v1-hosting-accounts-hostingaccountid-ftp-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

