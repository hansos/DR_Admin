# DnsBulkSyncResult

Result of a bulk DNS record sync operation across all domains for a registrar.

## Source

`DR_Admin/DTOs/DnsRecordSyncDto.cs`

## TypeScript Interface

```ts
export interface DnsBulkSyncResult {
  success: boolean;
  message: string;
  domainsProcessed: number;
  domainsSucceeded: number;
  domainsFailed: number;
  totalCreated: number;
  totalUpdated: number;
  totalSkipped: number;
  domainResults: DnsRecordSyncResult[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Success` | `bool` | `boolean` |
| `Message` | `string` | `string` |
| `DomainsProcessed` | `int` | `number` |
| `DomainsSucceeded` | `int` | `number` |
| `DomainsFailed` | `int` | `number` |
| `TotalCreated` | `int` | `number` |
| `TotalUpdated` | `int` | `number` |
| `TotalSkipped` | `int` | `number` |
| `DomainResults` | `List<DnsRecordSyncResult>` | `DnsRecordSyncResult[]` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
