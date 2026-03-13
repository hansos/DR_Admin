# POST CreateDnsZonePackageRecord

Creates a new DNS zone package record in the system

## Endpoint

```
POST /api/v1/dns-zone-package-records
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateDnsZonePackageRecordDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `DnsZonePackageRecordDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
