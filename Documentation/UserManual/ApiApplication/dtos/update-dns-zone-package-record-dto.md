# UpdateDnsZonePackageRecordDto

Data transfer object for updating an existing DNS zone package record

## Source

`DR_Admin/DTOs/DnsZonePackageRecordDto.cs`

## TypeScript Interface

```ts
export interface UpdateDnsZonePackageRecordDto {
  dnsRecordTypeId: number;
  name: string;
  value: string;
  valueSourceType: string;
  valueSourceReference: string | null;
  tTL: number;
  priority: number | null;
  weight: number | null;
  port: number | null;
  notes: string | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `DnsRecordTypeId` | `int` | `number` |
| `Name` | `string` | `string` |
| `Value` | `string` | `string` |
| `ValueSourceType` | `string` | `string` |
| `ValueSourceReference` | `string?` | `string | null` |
| `TTL` | `int` | `number` |
| `Priority` | `int?` | `number | null` |
| `Weight` | `int?` | `number | null` |
| `Port` | `int?` | `number | null` |
| `Notes` | `string?` | `string | null` |

## Used By Endpoints

- [PUT UpdateDnsZonePackageRecord](../dns-zone-package-records/put-update-dns-zone-package-record-api-v1-dns-zone-package-records-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

