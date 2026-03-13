# GET GetCustomerTaxProfileByCustomerId

Retrieves customer tax profile by customer ID

## Endpoint

```
GET /api/v1/customer-tax-profiles/customer/{customerId}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `CustomerTaxProfileDto` |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
