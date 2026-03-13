# POST CreateDnsZonePackage

Creates a new DNS zone package in the system

## Endpoint

```
POST /api/v1/dns-zone-packages
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | [CreateDnsZonePackageDto](../dtos/create-dns-zone-package-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [DnsZonePackageDto](../dtos/dns-zone-package-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




