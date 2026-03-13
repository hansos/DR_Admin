# POST CreatePaymentMethod

Creates a new payment method for a customer

## Endpoint

```
POST /api/v1/customer-payment-methods
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `[CreateCustomerPaymentMethodDto](../dtos/create-customer-payment-method-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[CustomerPaymentMethodDto](../dtos/customer-payment-method-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



