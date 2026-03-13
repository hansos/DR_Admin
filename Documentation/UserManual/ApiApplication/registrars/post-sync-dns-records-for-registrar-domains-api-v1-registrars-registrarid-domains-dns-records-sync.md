# POST SyncDnsRecordsForRegistrarDomains

Downloads DNS records from the registrar for all domains in the local database     and merges them into the DnsRecords table.

## Endpoint

```
POST /api/v1/registrars/{registrarId}/domains/dns-records/sync
```

## Authorization

Requires authentication. Policy: **Domain.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [DnsBulkSyncResult](../dtos/dns-bulk-sync-result.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




