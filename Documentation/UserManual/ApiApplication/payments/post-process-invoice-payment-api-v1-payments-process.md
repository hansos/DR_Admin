# POST ProcessInvoicePayment

Manages payment processing operations

## Endpoint

```
POST /api/v1/payments/process
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `[ProcessInvoicePaymentDto](../dtos/process-invoice-payment-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[PaymentResultDto](../dtos/payment-result-dto.md)` |
| 400 | Bad Request | - |

[Back to API Manual index](../index.md)



