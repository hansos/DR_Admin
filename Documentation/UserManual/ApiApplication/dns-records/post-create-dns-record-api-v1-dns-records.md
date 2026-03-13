# POST CreateDnsRecord

Retrieves all DNS records of a specific type (A, AAAA, CNAME, MX, TXT, NS, SRV, etc.).

## Endpoint

```
POST /api/v1/dns-records
```

## Authorization

Requires authentication. Policy: **DnsRecord.WriteOwn**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `[CreateDnsRecordDto](../dtos/create-dns-record-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[DnsRecordDto](../dtos/dns-record-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



