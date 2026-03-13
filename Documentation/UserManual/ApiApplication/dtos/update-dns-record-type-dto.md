# UpdateDnsRecordTypeDto

Data transfer object for updating an existing DNS record type

## Source

`DR_Admin/DTOs/DnsRecordTypeDto.cs`

## TypeScript Interface

```ts
export interface UpdateDnsRecordTypeDto {
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

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
