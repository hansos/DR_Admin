# POST RestoreDnsRecord

Restores a soft-deleted DNS record and marks it as pending synchronisation.

## Endpoint

```
POST /api/v1/dns-records/{id}/restore
```

## Authorization

Requires authentication. Policy: **DnsRecord.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [DnsRecordDto](../dtos/dns-record-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




