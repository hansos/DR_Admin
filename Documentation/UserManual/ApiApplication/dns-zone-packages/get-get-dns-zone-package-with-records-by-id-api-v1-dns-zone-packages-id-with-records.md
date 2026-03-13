# GET GetDnsZonePackageWithRecordsById

Retrieves a specific DNS zone package with its records by its unique identifier

## Endpoint

```
GET /api/v1/dns-zone-packages/{id}/with-records
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
| 200 | OK | `DnsZonePackageDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
