# POST MarkDnsRecordAsSynced

Clears the pending-sync flag on a DNS record after it has been successfully pushed to the DNS server.

## Endpoint

```
POST /api/v1/dns-records/{id}/mark-synced
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




