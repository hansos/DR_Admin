# POST CreateCustomerTaxProfile

Creates a new customer tax profile

## Endpoint

```
POST /api/v1/customer-tax-profiles
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `[CreateCustomerTaxProfileDto](../dtos/create-customer-tax-profile-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[CustomerTaxProfileDto](../dtos/customer-tax-profile-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



