# DnsPushPendingResult

Result of pushing all pending-sync DNS records for a domain to the registrar's DNS server.

## Source

`DR_Admin/DTOs/DnsRecordSyncDto.cs`

## TypeScript Interface

```ts
export interface DnsPushPendingResult {
  success: boolean;
  domainName: string;
  message: string;
  upserted: number;
  deleted: number;
  failed: number;
  recordResults: DnsPushRecordResult[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Success` | `bool` | `boolean` |
| `DomainName` | `string` | `string` |
| `Message` | `string` | `string` |
| `Upserted` | `int` | `number` |
| `Deleted` | `int` | `number` |
| `Failed` | `int` | `number` |
| `RecordResults` | `List<DnsPushRecordResult>` | `DnsPushRecordResult[]` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
