# POST ConfirmPaymentIntent

Confirms a payment intent with a payment method

## Endpoint

```
POST /api/v1/payment-intents/{id}/confirm
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `paymentMethodToken` | Body | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
