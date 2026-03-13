# GET GetPaymentIntentById

Manages payment intents for processing payments

## Endpoint

```
GET /api/v1/payment-intents/{id}
```

## Authorization

Requires authentication. Policy: **PaymentIntent.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[PaymentIntentDto](../dtos/payment-intent-dto.md)` |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



