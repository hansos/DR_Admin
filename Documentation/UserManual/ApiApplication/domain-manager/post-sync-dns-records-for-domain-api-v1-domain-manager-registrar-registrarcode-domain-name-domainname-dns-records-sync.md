# POST SyncDnsRecordsForDomain

Downloads DNS records from the registrar for a single domain and merges them into the local database

## Endpoint

```
POST /api/v1/domain-manager/registrar/{registrarCode}/domain/name/{domainName}/dns-records/sync
```

## Authorization

Requires authentication. Policy: **Domain.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarCode` | Route | `string` |
| `domainName` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[DnsRecordSyncResult](../dtos/dns-record-sync-result.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



