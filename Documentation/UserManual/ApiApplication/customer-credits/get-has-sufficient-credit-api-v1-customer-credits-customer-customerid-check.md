# GET HasSufficientCredit

Checks if a customer has sufficient credit

## Endpoint

```
GET /api/v1/customer-credits/customer/{customerId}/check
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Route | `int` |
| `amount` | Query | `decimal` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `bool` |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
