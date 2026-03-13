# GET GetDnsRecordTypeById

Manages DNS record types (A, AAAA, CNAME, MX, TXT, NS, SRV, etc.)

## Endpoint

```
GET /api/v1/dns-record-types/{id}
```

## Authorization

Requires authentication. Policy: **DnsRecord.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `DnsRecordTypeDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
