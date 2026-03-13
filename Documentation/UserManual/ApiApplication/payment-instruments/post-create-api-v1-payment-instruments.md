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
| `dto` | Body | `CreatePaymentInstrumentDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `PaymentInstrumentDto` |
| 400 | Bad Request | - |

[Back to API Manual index](../index.md)
