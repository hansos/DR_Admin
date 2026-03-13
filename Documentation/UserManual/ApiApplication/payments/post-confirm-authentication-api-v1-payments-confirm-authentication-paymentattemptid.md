# POST ConfirmAuthentication

Confirms payment authentication (3D Secure)

## Endpoint

```
POST /api/v1/payments/confirm-authentication/{paymentAttemptId}
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
| 200 | OK | [PaymentResultDto](../dtos/payment-result-dto.md) |

[Back to API Manual index](../index.md)




