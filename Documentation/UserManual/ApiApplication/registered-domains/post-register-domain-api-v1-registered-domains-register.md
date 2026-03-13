# POST RegisterDomain

Registers a new domain for the authenticated customer

## Endpoint

```
POST /api/v1/registered-domains/register
```

## Authorization

Requires authentication. Policy: **Domain.Register**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `RegisterDomainDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `DomainRegistrationResponseDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
