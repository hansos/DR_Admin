# CodeTablesResponseDto

Data transfer object for code tables check and update response

## Source

`DR_Admin/DTOs/CodeTablesResponseDto.cs`

## TypeScript Interface

```ts
export interface CodeTablesResponseDto {
  success: boolean;
  message: string;
  rolesAdded: number;
  customerStatusesAdded: number;
  dnsRecordTypesAdded: number;
  serviceTypesAdded: number;
  totalRoles: number;
  totalCustomerStatuses: number;
  totalDnsRecordTypes: number;
  totalServiceTypes: number;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Success` | `bool` | `boolean` |
| `Message` | `string` | `string` |
| `RolesAdded` | `int` | `number` |
| `CustomerStatusesAdded` | `int` | `number` |
| `DnsRecordTypesAdded` | `int` | `number` |
| `ServiceTypesAdded` | `int` | `number` |
| `TotalRoles` | `int` | `number` |
| `TotalCustomerStatuses` | `int` | `number` |
| `TotalDnsRecordTypes` | `int` | `number` |
| `TotalServiceTypes` | `int` | `number` |

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)
