# POST RetryFailedPayment

Retries a failed payment

## Endpoint

```
POST /api/v1/payments/retry/{paymentAttemptId}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `paymentAttemptId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `PaymentResultDto` |

[Back to API Manual index](../index.md)
