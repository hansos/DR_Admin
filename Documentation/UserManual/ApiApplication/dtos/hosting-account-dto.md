# HostingAccountDto

Data transfer object for HostingAccountDto.

## Source

`DR_Admin/DTOs/HostingAccountDto.cs`

## TypeScript Interface

```ts
export interface HostingAccountDto {
  id: number;
  customerId: number;
  customerName: string;
  serviceId: number;
  serverId: number | null;
  serverName: string | null;
  serverControlPanelId: number | null;
  controlPanelType: string | null;
  provider: string;
  username: string;
  status: string;
  expirationDate: string;
  externalAccountId: string | null;
  lastSyncedAt: string | null;
  syncStatus: string | null;
  planName: string | null;
  diskUsageMB: number | null;
  diskQuotaMB: number | null;
  bandwidthUsageMB: number | null;
  bandwidthLimitMB: number | null;
  maxEmailAccounts: number | null;
  maxDatabases: number | null;
  maxFtpAccounts: number | null;
  maxSubdomains: number | null;
  domains: HostingDomainDto[] | null;
  emailAccountCount: number | null;
  databaseCount: number | null;
  ftpAccountCount: number | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `CustomerId` | `int` | `number` |
| `CustomerName` | `string` | `string` |
| `ServiceId` | `int` | `number` |
| `ServerId` | `int?` | `number | null` |
| `ServerName` | `string?` | `string | null` |
| `ServerControlPanelId` | `int?` | `number | null` |
| `ControlPanelType` | `string?` | `string | null` |
| `Provider` | `string` | `string` |
| `Username` | `string` | `string` |
| `Status` | `string` | `string` |
| `ExpirationDate` | `DateTime` | `string` |
| `ExternalAccountId` | `string?` | `string | null` |
| `LastSyncedAt` | `DateTime?` | `string | null` |
| `SyncStatus` | `string?` | `string | null` |
| `PlanName` | `string?` | `string | null` |
| `DiskUsageMB` | `int?` | `number | null` |
| `DiskQuotaMB` | `int?` | `number | null` |
| `BandwidthUsageMB` | `int?` | `number | null` |
| `BandwidthLimitMB` | `int?` | `number | null` |
| `MaxEmailAccounts` | `int?` | `number | null` |
| `MaxDatabases` | `int?` | `number | null` |
| `MaxFtpAccounts` | `int?` | `number | null` |
| `MaxSubdomains` | `int?` | `number | null` |
| `Domains` | `List<HostingDomainDto>?` | `HostingDomainDto[] | null` |
| `EmailAccountCount` | `int?` | `number | null` |
| `DatabaseCount` | `int?` | `number | null` |
| `FtpAccountCount` | `int?` | `number | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetHostingAccount](../hosting-accounts/get-get-hosting-account-api-v1-hosting-accounts-id.md)
- [GET GetHostingAccountWithDetails](../hosting-accounts/get-get-hosting-account-with-details-api-v1-hosting-accounts-id-details.md)
- [POST CreateHostingAccountAndSync](../hosting-accounts/post-create-hosting-account-and-sync-api-v1-hosting-accounts-create-and-sync.md)
- [POST CreateHostingAccount](../hosting-accounts/post-create-hosting-account-api-v1-hosting-accounts.md)
- [PUT UpdateHostingAccount](../hosting-accounts/put-update-hosting-account-api-v1-hosting-accounts-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

