# POST CreatePaymentGateway

Creates a new payment gateway

## Endpoint

```
POST /api/v1/payment-gateways
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `CreatePaymentGatewayDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `PaymentGatewayDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
