# POST RegisterDomainForCustomer

Registers a new domain for a specific customer (Admin/Sales only)

## Endpoint

```
POST /api/v1/registered-domains/register-for-customer
```

## Authorization

Requires authentication. Policy: **Domain.RegisterForCustomer**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | [RegisterDomainForCustomerDto](../dtos/register-domain-for-customer-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [DomainRegistrationResponseDto](../dtos/domain-registration-response-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




