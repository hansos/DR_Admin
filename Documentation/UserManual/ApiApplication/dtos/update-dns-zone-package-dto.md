# UpdateDnsZonePackageDto

Data transfer object for updating an existing DNS zone package

## Source

`DR_Admin/DTOs/DnsZonePackageDto.cs`

## TypeScript Interface

```ts
export interface UpdateDnsZonePackageDto {
  name: string;
  description: string | null;
  isActive: boolean;
  isDefault: boolean;
  sortOrder: number;
  resellerCompanyId: number | null;
  salesAgentId: number | null;
}
```

## Fields

| Property | C# Type | TypeScript Type |
|----------|---------|-----------------|
| `Name` | `string` | `string` |
| `Description` | `string?` | `string | null` |
| `IsActive` | `bool` | `boolean` |
| `IsDefault` | `bool` | `boolean` |
| `SortOrder` | `int` | `number` |
| `ResellerCompanyId` | `int?` | `number | null` |
| `SalesAgentId` | `int?` | `number | null` |

## Used By Endpoints

- [PUT UpdateDnsZonePackage](../dns-zone-packages/put-update-dns-zone-package-api-v1-dns-zone-packages-id.md)

[Back to DTO index](index.md)

[Back to API Manual index](../index.md)

