# GET GetDefaultPaymentMethod

Retrieves the default payment method for a customer

## Endpoint

```
GET /api/v1/customer-payment-methods/customer/{customerId}/default
```

## Authorization

Requires authentication. Policy: **CustomerPaymentMethod.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[CustomerPaymentMethodDto](../dtos/customer-payment-method-dto.md)` |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



