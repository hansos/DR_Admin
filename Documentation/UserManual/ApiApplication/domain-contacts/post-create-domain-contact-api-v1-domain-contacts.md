# POST CreateDomainContact

Creates a new domain contact in the system

## Endpoint

```
POST /api/v1/domain-contacts
```

## Authorization

Requires authentication. Policy: **Domain.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `[CreateDomainContactDto](../dtos/create-domain-contact-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[DomainContactDto](../dtos/domain-contact-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



