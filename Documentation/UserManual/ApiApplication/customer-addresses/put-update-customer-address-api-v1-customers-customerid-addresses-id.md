# PUT UpdateCustomerAddress

Updates an existing customer address

## Endpoint

```
PUT /api/v1/customers/{customerId}/addresses/{id}
```

## Authorization

Requires authentication. Policy: **Customer.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Route | `int` |
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateCustomerAddressDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `CustomerAddressDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
