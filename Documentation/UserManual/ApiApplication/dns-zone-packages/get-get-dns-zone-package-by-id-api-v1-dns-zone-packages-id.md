# GET GetDnsZonePackageById

Retrieves a specific DNS zone package by its unique identifier

## Endpoint

```
GET /api/v1/dns-zone-packages/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [DnsZonePackageDto](../dtos/dns-zone-package-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




