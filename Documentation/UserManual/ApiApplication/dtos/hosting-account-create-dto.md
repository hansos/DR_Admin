# HostingAccountCreateDto

Data transfer object for HostingAccountCreateDto.

## Source

`DR_Admin/DTOs/HostingAccountDto.cs`

## TypeScript Interface

```ts
export interface HostingAccountCreateDto {
  customerId: number;
  serviceId: number;
  serverId: number | null;
  serverControlPanelId: number | null;
  provider: string | null;
  username: string;
  password: string;
  status: string | null;
  expirationDate: string;
  planName: string | null;
  diskQuotaMB: number | null;
  bandwidthLimitMB: number | null;
  maxEmailAccounts: number | null;
  maxDatabases: number | null;
  maxFtpAccounts: number | null;
  maxSubdomains: number | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `CustomerId` | `int` | `number` |
| `ServiceId` | `int` | `number` |
| `ServerId` | `int?` | `number | null` |
| `ServerControlPanelId` | `int?` | `number | null` |
| `Provider` | `string?` | `string | null` |
| `Username` | `string` | `string` |
| `Password` | `string` | `string` |
| `Status` | `string?` | `string | null` |
| `ExpirationDate` | `DateTime` | `string` |
| `PlanName` | `string?` | `string | null` |
| `DiskQuotaMB` | `int?` | `number | null` |
| `BandwidthLimitMB` | `int?` | `number | null` |
| `MaxEmailAccounts` | `int?` | `number | null` |
| `MaxDatabases` | `int?` | `number | null` |
| `MaxFtpAccounts` | `int?` | `number | null` |
| `MaxSubdomains` | `int?` | `number | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
