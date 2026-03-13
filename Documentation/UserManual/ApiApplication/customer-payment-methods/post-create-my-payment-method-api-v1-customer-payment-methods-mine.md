# POST CreateMyPaymentMethod

Manages customer payment methods

## Endpoint

```
POST /api/v1/customer-payment-methods/mine
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateCustomerPaymentMethodDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `CustomerPaymentMethodDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
