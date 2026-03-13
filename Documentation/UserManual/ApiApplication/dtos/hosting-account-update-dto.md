# HostingAccountUpdateDto

Data transfer object for HostingAccountUpdateDto.

## Source

`DR_Admin/DTOs/HostingAccountDto.cs`

## TypeScript Interface

```ts
export interface HostingAccountUpdateDto {
  status: string | null;
  password: string | null;
  planName: string | null;
  diskQuotaMB: number | null;
  bandwidthLimitMB: number | null;
  maxEmailAccounts: number | null;
  maxDatabases: number | null;
  maxFtpAccounts: number | null;
  maxSubdomains: number | null;
  expirationDate: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Status` | `string?` | `string | null` |
| `Password` | `string?` | `string | null` |
| `PlanName` | `string?` | `string | null` |
| `DiskQuotaMB` | `int?` | `number | null` |
| `BandwidthLimitMB` | `int?` | `number | null` |
| `MaxEmailAccounts` | `int?` | `number | null` |
| `MaxDatabases` | `int?` | `number | null` |
| `MaxFtpAccounts` | `int?` | `number | null` |
| `MaxSubdomains` | `int?` | `number | null` |
| `ExpirationDate` | `DateTime?` | `string | null` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
