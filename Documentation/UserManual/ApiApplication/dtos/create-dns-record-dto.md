# CreateDnsRecordDto

Data transfer object for creating a new DNS record.

## Source

`DR_Admin/DTOs/DnsRecordDto.cs`

## TypeScript Interface

```ts
export interface CreateDnsRecordDto {
  domainId: number;
  dnsRecordTypeId: number;
  name: string;
  value: string;
  tTL: number;
  priority: number | null;
  weight: number | null;
  port: number | null;
  isPendingSync: boolean;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `DomainId` | `int` | `number` |
| `DnsRecordTypeId` | `int` | `number` |
| `Name` | `string` | `string` |
| `Value` | `string` | `string` |
| `TTL` | `int` | `number` |
| `Priority` | `int?` | `number | null` |
| `Weight` | `int?` | `number | null` |
| `Port` | `int?` | `number | null` |
| `IsPendingSync` | `bool` | `boolean` |

## Used By Endpoints

- [POST CreateDnsRecord](../dns-records/post-create-dns-record-api-v1-dns-records.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

