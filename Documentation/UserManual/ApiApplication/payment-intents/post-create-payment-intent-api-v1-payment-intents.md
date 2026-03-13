# POST CreatePaymentIntent

Retrieves all payment intents for a specific customer

## Endpoint

```
POST /api/v1/payment-intents
```

## Authorization

Requires authentication. Policy: **PaymentIntent.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreatePaymentIntentDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `PaymentIntentDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
