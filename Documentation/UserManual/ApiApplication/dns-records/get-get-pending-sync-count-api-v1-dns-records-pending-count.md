# GET GetPendingSyncCount

Retrieves all non-deleted DNS records for a specific domain.

## Endpoint

```
GET /api/v1/dns-records/pending/count
```

## Authorization

Requires authentication. Policy: **DnsRecord.Read**.

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `object` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
