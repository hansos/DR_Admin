# GET GetDefaultDnsZonePackage

Manages DNS zone packages including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/dns-zone-packages/default
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[DnsZonePackageDto](../dtos/dns-zone-package-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



