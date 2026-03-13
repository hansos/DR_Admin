# GET GetDnsRecordById

Retrieves a specific DNS record by its unique identifier.

## Endpoint

```
GET /api/v1/dns-records/{id}
```

## Authorization

Requires authentication. Policy: **DnsRecord.ReadOwn**.

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




