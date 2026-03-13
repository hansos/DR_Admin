# PUT UpdateDnsZonePackageRecord

Updates an existing DNS zone package record's information

## Endpoint

```
PUT /api/v1/dns-zone-package-records/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateDnsZonePackageRecordDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `DnsZonePackageRecordDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
