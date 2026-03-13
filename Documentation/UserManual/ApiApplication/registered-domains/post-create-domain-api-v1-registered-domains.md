# POST CreateDomain

Creates a new domain

## Endpoint

```
POST /api/v1/registered-domains
```

## Authorization

Requires authentication. Policy: **Domain.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateRegisteredDomainDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `RegisteredDomainDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
