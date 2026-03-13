# GET GetAllCustomers

Retrieves tracked changes for a customer.

## Endpoint

```
GET /api/v1/customers
```

## Authorization

Requires authentication. Policy: **Customer.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | IEnumerable<[CustomerDto](../dtos/customer-dto.md)> |
| 200 | OK | [PagedResult](../dtos/paged-result.md)<[CustomerDto](../dtos/customer-dto.md)> |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




