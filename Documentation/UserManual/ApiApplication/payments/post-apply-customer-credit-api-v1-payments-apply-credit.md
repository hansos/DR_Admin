# POST ApplyCustomerCredit

Applies customer credit to an invoice

## Endpoint

```
POST /api/v1/payments/apply-credit
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `ApplyCustomerCreditDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `PaymentResultDto` |

[Back to API Manual index](../index.md)
