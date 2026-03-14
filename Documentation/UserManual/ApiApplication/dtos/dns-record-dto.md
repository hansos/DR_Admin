# DnsRecordDto

Data transfer object representing a DNS record for a domain.

## Source

`DR_Admin/DTOs/DnsRecordDto.cs`

## TypeScript Interface

```ts
export interface DnsRecordDto {
  id: number;
  domainId: number;
  dnsRecordTypeId: number;
  type: string;
  name: string;
  value: string;
  tTL: number;
  priority: number | null;
  weight: number | null;
  port: number | null;
  isEditableByUser: boolean;
  hasPriority: boolean;
  hasWeight: boolean;
  hasPort: boolean;
  isPendingSync: boolean;
  isDeleted: boolean;
  deletedAt: string | null;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `DomainId` | `int` | `number` |
| `DnsRecordTypeId` | `int` | `number` |
| `Type` | `string` | `string` |
| `Name` | `string` | `string` |
| `Value` | `string` | `string` |
| `TTL` | `int` | `number` |
| `Priority` | `int?` | `number | null` |
| `Weight` | `int?` | `number | null` |
| `Port` | `int?` | `number | null` |
| `IsEditableByUser` | `bool` | `boolean` |
| `HasPriority` | `bool` | `boolean` |
| `HasWeight` | `bool` | `boolean` |
| `HasPort` | `bool` | `boolean` |
| `IsPendingSync` | `bool` | `boolean` |
| `IsDeleted` | `bool` | `boolean` |
| `DeletedAt` | `DateTime?` | `string | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

## Used By Endpoints

- [GET GetAllDnsRecords](../dns-records/get-get-all-dns-records-api-v1-dns-records.md)
- [GET GetDnsRecordById](../dns-records/get-get-dns-record-by-id-api-v1-dns-records-id.md)
- [POST CreateDnsRecord](../dns-records/post-create-dns-record-api-v1-dns-records.md)
- [POST MarkDnsRecordAsSynced](../dns-records/post-mark-dns-record-as-synced-api-v1-dns-records-id-mark-synced.md)
- [POST RestoreDnsRecord](../dns-records/post-restore-dns-record-api-v1-dns-records-id-restore.md)
- [PUT UpdateDnsRecord](../dns-records/put-update-dns-record-api-v1-dns-records-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

