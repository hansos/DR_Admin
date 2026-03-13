# HostingPackageDto

Data transfer object representing a hosting package

## Source

`DR_Admin/DTOs/HostingPackageDto.cs`

## TypeScript Interface

```ts
export interface HostingPackageDto {
  id: number;
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
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
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
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
