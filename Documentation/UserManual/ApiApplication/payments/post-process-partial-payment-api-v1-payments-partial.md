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
| `dto` | Body | `[ProcessPartialPaymentDto](../dtos/process-partial-payment-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[PaymentResultDto](../dtos/payment-result-dto.md)` |

[Back to API Manual index](../index.md)



