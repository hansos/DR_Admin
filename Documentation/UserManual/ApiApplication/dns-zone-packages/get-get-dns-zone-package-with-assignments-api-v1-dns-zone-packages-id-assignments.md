# GET GetDnsZonePackageWithAssignments

GET GetDnsZonePackageWithAssignments

## Endpoint

```
GET /api/v1/dns-zone-packages/{id}/assignments
```

## Authorization

Requires authentication. Policy: **DnsZonePackage.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[DnsZonePackageDto](../dtos/dns-zone-package-dto.md)` |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



