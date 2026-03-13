# PUT UpdateDomain

Updates an existing domain

## Endpoint

```
PUT /api/v1/registered-domains/{id}
```

## Authorization

Requires authentication. Policy: **Domain.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateRegisteredDomainDto](../dtos/update-registered-domain-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [RegisteredDomainDto](../dtos/registered-domain-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




