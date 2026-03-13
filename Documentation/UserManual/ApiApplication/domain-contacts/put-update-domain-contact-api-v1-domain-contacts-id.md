# PUT UpdateDomainContact

Updates an existing domain contact's information

## Endpoint

```
PUT /api/v1/domain-contacts/{id}
```

## Authorization

Requires authentication. Policy: **Domain.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `[UpdateDomainContactDto](../dtos/update-domain-contact-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[DomainContactDto](../dtos/domain-contact-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



