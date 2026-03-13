# PUT UpdatePaymentMethod

Retrieves all payment methods for a customer

## Endpoint

```
PUT /api/v1/customer-payment-methods/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `customerId` | Query | `int` |
| `updateDto` | Body | `[UpdateCustomerPaymentMethodDto](../dtos/update-customer-payment-method-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[CustomerPaymentMethodDto](../dtos/customer-payment-method-dto.md)` |
| 400 | Bad Request | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



