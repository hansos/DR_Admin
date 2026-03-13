# GET GetDnsRecordTypeByType

Retrieves a specific DNS record type by its type name (e.g., A, AAAA, CNAME, MX, TXT)

## Endpoint

```
GET /api/v1/dns-record-types/type/{type}
```

## Authorization

Requires authentication. Policy: **DnsRecordType.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `type` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[DnsRecordTypeDto](../dtos/dns-record-type-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



