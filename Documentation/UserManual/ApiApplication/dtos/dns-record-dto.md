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

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
