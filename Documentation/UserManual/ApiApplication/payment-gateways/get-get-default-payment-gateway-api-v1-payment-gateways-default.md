# GET GetDefaultPaymentGateway

Manages payment gateway configurations including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/payment-gateways/default
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[PaymentGatewayDto](../dtos/payment-gateway-dto.md)` |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



