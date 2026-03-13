# PATCH PatchContactPersonIsDomainGlobal

Updates the IsDomainGlobal flag for an existing contact person

## Endpoint

```
PATCH /api/v1/contact-persons/{id}/domain-global
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `[UpdateContactPersonIsDomainGlobalDto](../dtos/update-contact-person-is-domain-global-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[ContactPersonDto](../dtos/contact-person-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



