# DELETE DeleteDnsRecord

Soft-deletes a DNS record by flagging it as deleted and pending synchronisation.     The record is retained until hard-deleted after the removal is confirmed on the DNS server.

## Endpoint

```
DELETE /api/v1/dns-records/{id}
```

## Authorization

Requires authentication. Policy: **DnsRecord.Delete**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
