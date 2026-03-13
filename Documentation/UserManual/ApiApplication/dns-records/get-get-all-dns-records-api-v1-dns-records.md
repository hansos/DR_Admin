# GET GetAllDnsRecords

Manages DNS records for domains.

## Endpoint

```
GET /api/v1/dns-records
```

## Authorization

Requires authentication. Policy: **DnsRecord.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `IEnumerable<DnsRecordDto>` |
| 200 | OK | `PagedResult<DnsRecordDto>` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
