# DELETE HardDeleteDnsRecord

Permanently removes a DNS record from the database.     Use this endpoint only after the deletion has been confirmed on the DNS server.

## Endpoint

```
DELETE /api/v1/dns-records/{id}/hard
```

## Authorization

Requires authentication. Policy: **DnsRecord.HardDelete**.

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
