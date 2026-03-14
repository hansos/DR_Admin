# CreateDnsRecordTypeDto

Data transfer object for creating a new DNS record type

## Source

`DR_Admin/DTOs/DnsRecordTypeDto.cs`

## TypeScript Interface

```ts
export interface CreateDnsRecordTypeDto {
  type: string;
  description: string;
  hasPriority: boolean;
  hasWeight: boolean;
  hasPort: boolean;
  isEditableByUser: boolean;
  isActive: boolean;
  defaultTTL: number;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Type` | `string` | `string` |
| `Description` | `string` | `string` |
| `HasPriority` | `bool` | `boolean` |
| `HasWeight` | `bool` | `boolean` |
| `HasPort` | `bool` | `boolean` |
| `IsEditableByUser` | `bool` | `boolean` |
| `IsActive` | `bool` | `boolean` |
| `DefaultTTL` | `int` | `number` |

## Used By Endpoints

- [POST CreateDnsRecordType](../dns-record-types/post-create-dns-record-type-api-v1-dns-record-types.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

