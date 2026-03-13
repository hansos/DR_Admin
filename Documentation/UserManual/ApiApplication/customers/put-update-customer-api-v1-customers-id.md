# PUT UpdateCustomer

Updates an existing customer's information

## Endpoint

```
PUT /api/v1/customers/{id}
```

## Authorization

Requires authentication. Policy: **Customer.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateCustomerDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `CustomerDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
