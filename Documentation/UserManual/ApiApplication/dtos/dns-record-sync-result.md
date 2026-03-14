# DnsRecordSyncResult

Result of a DNS record sync operation for a single domain.

## Source

`DR_Admin/DTOs/DnsRecordSyncDto.cs`

## TypeScript Interface

```ts
export interface DnsRecordSyncResult {
  domainName: string;
  success: boolean;
  errorMessage: string | null;
  created: number;
  updated: number;
  skipped: number;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `DomainName` | `string` | `string` |
| `Success` | `bool` | `boolean` |
| `ErrorMessage` | `string?` | `string | null` |
| `Created` | `int` | `number` |
| `Updated` | `int` | `number` |
| `Skipped` | `int` | `number` |

## Used By Endpoints

- [POST SyncDnsRecordsForDomain](../domain-manager/post-sync-dns-records-for-domain-api-v1-domain-manager-registrar-registrarcode-domain-name-domainname-dns-records-sync.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

