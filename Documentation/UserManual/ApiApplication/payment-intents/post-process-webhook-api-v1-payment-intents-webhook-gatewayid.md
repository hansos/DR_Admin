# POST ProcessWebhook

Processes a webhook from a payment gateway

## Endpoint

```
POST /api/v1/payment-intents/webhook/{gatewayId}
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `gatewayId` | Route | `int` |
| `payload` | Body | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 400 | Bad Request | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
