# PUT UpdateDnsRecord

Updates an existing DNS record. The record is automatically marked as pending synchronisation.     System-managed records (IsEditableByUser = false) can only be edited by Admin or Support.

## Endpoint

```
PUT /api/v1/dns-records/{id}
```

## Authorization

Requires authentication. Policy: **DnsRecord.WriteOwn**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `[UpdateDnsRecordDto](../dtos/update-dns-record-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[DnsRecordDto](../dtos/dns-record-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



