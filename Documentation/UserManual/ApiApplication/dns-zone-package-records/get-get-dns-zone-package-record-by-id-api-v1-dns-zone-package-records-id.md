# GET GetDnsZonePackageRecordById

Manages DNS zone package records including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/dns-zone-package-records/{id}
```

## Authorization

Requires authentication. Policy: **DnsRecord.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [DnsZonePackageRecordDto](../dtos/dns-zone-package-record-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




