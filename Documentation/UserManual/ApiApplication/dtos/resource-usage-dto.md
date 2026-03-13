# ResourceUsageDto

Data transfer object for ResourceUsageDto.

## Source

`DR_Admin/DTOs/HostingAccountDto.cs`

## TypeScript Interface

```ts
export interface ResourceUsageDto {
  hostingAccountId: number;
  diskUsageMB: number | null;
  diskQuotaMB: number | null;
  bandwidthUsageMB: number | null;
  bandwidthLimitMB: number | null;
  emailAccountCount: number;
  maxEmailAccounts: number | null;
  databaseCount: number;
  maxDatabases: number | null;
  ftpAccountCount: number;
  maxFtpAccounts: number | null;
  domainCount: number;
  maxSubdomains: number | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `HostingAccountId` | `int` | `number` |
| `DiskUsageMB` | `int?` | `number | null` |
| `DiskQuotaMB` | `int?` | `number | null` |
| `BandwidthUsageMB` | `int?` | `number | null` |
| `BandwidthLimitMB` | `int?` | `number | null` |
| `EmailAccountCount` | `int` | `number` |
| `MaxEmailAccounts` | `int?` | `number | null` |
| `DatabaseCount` | `int` | `number` |
| `MaxDatabases` | `int?` | `number | null` |
| `FtpAccountCount` | `int` | `number` |
| `MaxFtpAccounts` | `int?` | `number | null` |
| `DomainCount` | `int` | `number` |
| `MaxSubdomains` | `int?` | `number | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
