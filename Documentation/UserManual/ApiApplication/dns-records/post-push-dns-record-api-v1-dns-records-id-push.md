# POST PushDnsRecord

Pushes a single DNS record to the registrar's DNS server.     Non-deleted records are upserted (created or updated); soft-deleted records are removed     from the server and permanently deleted locally.     IsPendingSync is cleared on success.

## Endpoint

```
POST /api/v1/dns-records/{id}/push
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
| 200 | OK | `DnsPushRecordResult` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
