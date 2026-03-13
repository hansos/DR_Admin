# PUT UpdatePaymentGateway

Updates an existing payment gateway

## Endpoint

```
PUT /api/v1/payment-gateways/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `dto` | Body | [UpdatePaymentGatewayDto](../dtos/update-payment-gateway-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [PaymentGatewayDto](../dtos/payment-gateway-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




