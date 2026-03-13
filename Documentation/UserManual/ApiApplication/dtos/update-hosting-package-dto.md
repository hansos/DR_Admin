# UpdateHostingPackageDto

Data transfer object for updating an existing hosting package

## Source

`DR_Admin/DTOs/HostingPackageDto.cs`

## TypeScript Interface

```ts
export interface UpdateHostingPackageDto {
  name: string;
  description: string | null;
  diskSpaceMB: number;
  bandwidthMB: number;
  emailAccounts: number;
  databases: number;
  domains: number;
  subdomains: number;
  ftpAccounts: number;
  sslSupport: boolean;
  backupSupport: boolean;
  dedicatedIp: boolean;
  monthlyPrice: number;
  yearlyPrice: number;
  isActive: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `Description` | `string?` | `string | null` |
| `DiskSpaceMB` | `int` | `number` |
| `BandwidthMB` | `int` | `number` |
| `EmailAccounts` | `int` | `number` |
| `Databases` | `int` | `number` |
| `Domains` | `int` | `number` |
| `Subdomains` | `int` | `number` |
| `FtpAccounts` | `int` | `number` |
| `SslSupport` | `bool` | `boolean` |
| `BackupSupport` | `bool` | `boolean` |
| `DedicatedIp` | `bool` | `boolean` |
| `MonthlyPrice` | `decimal` | `number` |
| `YearlyPrice` | `decimal` | `number` |
| `IsActive` | `bool` | `boolean` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
