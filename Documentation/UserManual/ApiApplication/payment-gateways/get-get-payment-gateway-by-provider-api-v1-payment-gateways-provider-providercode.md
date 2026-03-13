# GET GetPaymentGatewayByProvider

Retrieves a payment gateway by provider code

## Endpoint

```
GET /api/v1/payment-gateways/provider/{providerCode}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `providerCode` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `PaymentGatewayDto` |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
