# UpdateDnsRecordDto

Data transfer object for updating an existing DNS record. Updating a record automatically marks it as pending synchronisation.

## Source

`DR_Admin/DTOs/DnsRecordDto.cs`

## TypeScript Interface

```ts
export interface UpdateDnsRecordDto {
  domainId: number;
  dnsRecordTypeId: number;
  name: string;
  value: string;
  tTL: number;
  priority: number | null;
  weight: number | null;
  port: number | null;
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

## Used By Endpoints

- [PUT UpdateDnsRecord](../dns-records/put-update-dns-record-api-v1-dns-records-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

