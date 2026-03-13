# PUT UpdateDnsRecordType

Update an existing DNS record type

## Endpoint

```
PUT /api/v1/dns-record-types/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `[UpdateDnsRecordTypeDto](../dtos/update-dns-record-type-dto.md)` |

[Back to API Manual index](../index.md)



