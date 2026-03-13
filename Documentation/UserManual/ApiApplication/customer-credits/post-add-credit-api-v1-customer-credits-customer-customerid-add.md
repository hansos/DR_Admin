# POST AddCredit

Adds credit to a customer account

## Endpoint

```
POST /api/v1/customer-credits/customer/{customerId}/add
```

## Authorization

Requires authentication. Policy: **CustomerCredit.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Route | `int` |
| `amount` | Query | `decimal` |
| `description` | Query | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `decimal` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
