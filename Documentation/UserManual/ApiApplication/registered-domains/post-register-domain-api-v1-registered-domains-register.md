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
| `dto` | Body | [RegisterDomainDto](../dtos/register-domain-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [DomainRegistrationResponseDto](../dtos/domain-registration-response-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




