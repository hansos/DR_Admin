# GET GetById

GET GetById

## Endpoint

```
GET /api/v1/payment-instruments/{id}
```

## Authorization

Requires authentication. Policy: **PaymentGateway.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `PaymentInstrumentDto` |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)
