# POST HandleWebhook

Handles payment gateway webhooks

## Endpoint

```
POST /api/v1/payments/webhook/{gatewayName}
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `gatewayName` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |

[Back to API Manual index](../index.md)
