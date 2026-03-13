# POST PushPendingSyncDnsRecords

Pushes all pending-sync DNS records for a domain to the registrar's DNS server.     Non-deleted records are upserted; soft-deleted records are removed from the server     and permanently deleted locally.

## Endpoint

```
POST /api/v1/dns-records/domain/{domainId}/push-pending
```

## Authorization

Requires authentication. Policy: **DnsRecord.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `domainId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `DnsPushPendingResult` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
