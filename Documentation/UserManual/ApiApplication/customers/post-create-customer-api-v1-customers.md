# POST CreateCustomer

Creates a new customer in the system

## Endpoint

```
POST /api/v1/customers
```

## Authorization

Requires authentication. Policy: **Customer.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | [CreateCustomerDto](../dtos/create-customer-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [CustomerDto](../dtos/customer-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




