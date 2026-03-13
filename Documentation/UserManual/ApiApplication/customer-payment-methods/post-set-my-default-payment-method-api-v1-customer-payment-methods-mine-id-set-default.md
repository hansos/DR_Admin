# POST SetMyDefaultPaymentMethod

POST SetMyDefaultPaymentMethod

## Endpoint

```
POST /api/v1/customer-payment-methods/mine/{id}/set-default
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
