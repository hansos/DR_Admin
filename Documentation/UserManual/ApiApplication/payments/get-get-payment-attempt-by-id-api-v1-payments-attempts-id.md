# GET GetPaymentAttemptById

Gets payment attempts for an invoice

## Endpoint

```
GET /api/v1/payments/attempts/{id}
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
| 200 | OK | [PaymentAttemptDto](../dtos/payment-attempt-dto.md) |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)




