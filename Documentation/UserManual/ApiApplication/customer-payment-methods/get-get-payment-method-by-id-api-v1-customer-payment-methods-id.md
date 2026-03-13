# GET GetPaymentMethodById

Retrieves a specific payment method by ID

## Endpoint

```
GET /api/v1/customer-payment-methods/{id}
```

## Authorization

Requires authentication. Policy: **CustomerPaymentMethod.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[CustomerPaymentMethodDto](../dtos/customer-payment-method-dto.md)` |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



