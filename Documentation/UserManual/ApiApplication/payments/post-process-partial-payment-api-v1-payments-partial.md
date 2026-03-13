# POST ProcessPartialPayment

Processes a partial payment

## Endpoint

```
POST /api/v1/payments/partial
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `ProcessPartialPaymentDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `PaymentResultDto` |

[Back to API Manual index](../index.md)
