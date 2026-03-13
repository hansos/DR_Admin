# POST CreateCreditTransaction

Retrieves all credit transactions for a customer

## Endpoint

```
POST /api/v1/customer-credits/transactions
```

## Authorization

Requires authentication. Policy: **CustomerCredit.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateCreditTransactionDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `CreditTransactionDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
