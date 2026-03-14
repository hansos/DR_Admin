# DnsZonePackageDto

Data transfer object representing a DNS zone package

## Source

`DR_Admin/DTOs/DnsZonePackageDto.cs`

## TypeScript Interface

```ts
export interface DnsZonePackageDto {
  id: number;
  name: string;
  description: string | null;
  isActive: boolean;
  isDefault: boolean;
  sortOrder: number;
  resellerCompanyId: number | null;
  salesAgentId: number | null;
  createdAt: string;
  updatedAt: string;
  records: DnsZonePackageRecordDto[];
  controlPanels: DnsZonePackageControlPanelSummaryDto[];
  servers: DnsZonePackageServerSummaryDto[];
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Id` | `int` | `number` |
| `Name` | `string` | `string` |
| `Description` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `IsDefault` | `bool` | `boolean` |
| `SortOrder` | `int` | `number` |
| `ResellerCompanyId` | `int?` | `number | null` |
| `SalesAgentId` | `int?` | `number | null` |
| `CreatedAt` | `DateTime` | `string` |
| `UpdatedAt` | `DateTime` | `string` |
| `Records` | `ICollection<DnsZonePackageRecordDto>` | `DnsZonePackageRecordDto[]` |
| `ControlPanels` | `ICollection<DnsZonePackageControlPanelSummaryDto>` | `DnsZonePackageControlPanelSummaryDto[]` |
| `Servers` | `ICollection<DnsZonePackageServerSummaryDto>` | `DnsZonePackageServerSummaryDto[]` |

## Used By Endpoints

- [GET GetDefaultDnsZonePackage](../dns-zone-packages/get-get-default-dns-zone-package-api-v1-dns-zone-packages-default.md)
- [GET GetDnsZonePackageById](../dns-zone-packages/get-get-dns-zone-package-by-id-api-v1-dns-zone-packages-id.md)
- [GET GetDnsZonePackageWithAssignments](../dns-zone-packages/get-get-dns-zone-package-with-assignments-api-v1-dns-zone-packages-id-assignments.md)
- [GET GetDnsZonePackageWithRecordsById](../dns-zone-packages/get-get-dns-zone-package-with-records-by-id-api-v1-dns-zone-packages-id-with-records.md)
- [POST CreateDnsZonePackage](../dns-zone-packages/post-create-dns-zone-package-api-v1-dns-zone-packages.md)
- [PUT UpdateDnsZonePackage](../dns-zone-packages/put-update-dns-zone-package-api-v1-dns-zone-packages-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

