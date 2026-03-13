# PUT Update

PUT Update

## Endpoint

```
PUT /api/v1/payment-instruments/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `dto` | Body | [UpdatePaymentInstrumentDto](../dtos/update-payment-instrument-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [PaymentInstrumentDto](../dtos/payment-instrument-dto.md) |
| 404 | Not Found | - |
| 400 | Bad Request | - |

[Back to API Manual index](../index.md)




