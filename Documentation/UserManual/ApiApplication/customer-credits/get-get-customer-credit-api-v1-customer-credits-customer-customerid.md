# GET GetCustomerCredit

Manages customer credit balances and transactions

## Endpoint

```
GET /api/v1/customer-credits/customer/{customerId}
```

## Authorization

Requires authentication. Policy: **CustomerCredit.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [CustomerCreditDto](../dtos/customer-credit-dto.md) |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




