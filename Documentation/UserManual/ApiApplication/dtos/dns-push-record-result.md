# DnsPushRecordResult

Result of pushing a single DNS record to the registrar's DNS server.

## Source

`DR_Admin/DTOs/DnsRecordSyncDto.cs`

## TypeScript Interface

```ts
export interface DnsPushRecordResult {
  success: boolean;
  dnsRecordId: number;
  message: string;
  action: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Success` | `bool` | `boolean` |
| `DnsRecordId` | `int` | `number` |
| `Message` | `string` | `string` |
| `Action` | `string` | `string` |

## Used By Endpoints

- [POST PushDnsRecord](../dns-records/post-push-dns-record-api-v1-dns-records-id-push.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

