# POST SyncDnsRecordsForAllDomains

Downloads DNS records from the registrar for all domains assigned to that registrar     and merges them into the local database

## Endpoint

```
POST /api/v1/domain-manager/registrar/{registrarCode}/dns-records/sync
```

## Authorization

Requires authentication. Policy: **Domain.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarCode` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[DnsBulkSyncResult](../dtos/dns-bulk-sync-result.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



