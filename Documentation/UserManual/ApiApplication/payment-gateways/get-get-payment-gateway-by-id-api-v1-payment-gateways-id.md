# GET GetPaymentGatewayById

Retrieves a specific payment gateway by its unique identifier

## Endpoint

```
GET /api/v1/payment-gateways/{id}
```

## Authorization

Requires authentication. Policy: **PaymentGateway.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `PaymentGatewayDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
