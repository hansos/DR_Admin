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
| `dto` | Body | `[ApplyCustomerCreditDto](../dtos/apply-customer-credit-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[PaymentResultDto](../dtos/payment-result-dto.md)` |

[Back to API Manual index](../index.md)



