# DnsZonePackageRecordDto

Data transfer object representing a DNS zone package record

## Source

`DR_Admin/DTOs/DnsZonePackageRecordDto.cs`

## TypeScript Interface

```ts
export interface DnsZonePackageRecordDto {
  id: number;
  dnsZonePackageId: number;
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
  createdAt: string;
  updatedAt: string;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `DnsZonePackageId` | `int` | `number` |
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
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
