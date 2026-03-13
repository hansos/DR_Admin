# POST ApplyPackageToDomain

Applies a DNS zone package to a domain by creating DNS records

## Endpoint

```
POST /api/v1/dns-zone-packages/{packageId}/apply-to-domain/{domainId}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `packageId` | Route | `int` |
| `domainId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
