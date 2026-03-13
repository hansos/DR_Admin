# DnsRecordTypeDto

Data transfer object representing a DNS record type (e.g., A, AAAA, CNAME, MX, TXT)

## Source

`DR_Admin/DTOs/DnsRecordTypeDto.cs`

## TypeScript Interface

```ts
export interface DnsRecordTypeDto {
  id: number;
  type: string;
  description: string;
  hasPriority: boolean;
  hasWeight: boolean;
  hasPort: boolean;
  isEditableByUser: boolean;
  isActive: boolean;
  defaultTTL: number;
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Type` | `string` | `string` |
| `Description` | `string` | `string` |
| `HasPriority` | `bool` | `boolean` |
| `HasWeight` | `bool` | `boolean` |
| `HasPort` | `bool` | `boolean` |
| `IsEditableByUser` | `bool` | `boolean` |
| `IsActive` | `bool` | `boolean` |
| `DefaultTTL` | `int` | `number` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
