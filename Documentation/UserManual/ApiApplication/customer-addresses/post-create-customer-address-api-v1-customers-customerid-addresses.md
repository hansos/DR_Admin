# POST CreateCustomerAddress

Creates a new customer address

## Endpoint

```
POST /api/v1/customers/{customerId}/addresses
```

## Authorization

Requires authentication. Policy: **Customer.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Route | `int` |
| `createDto` | Body | `CreateCustomerAddressDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `CustomerAddressDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
