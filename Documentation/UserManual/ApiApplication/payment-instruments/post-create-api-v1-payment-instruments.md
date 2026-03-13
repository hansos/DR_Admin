# POST Create

POST Create

## Endpoint

```
POST /api/v1/payment-instruments
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `[CreatePaymentInstrumentDto](../dtos/create-payment-instrument-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[PaymentInstrumentDto](../dtos/payment-instrument-dto.md)` |
| 400 | Bad Request | - |

[Back to API Manual index](../index.md)



