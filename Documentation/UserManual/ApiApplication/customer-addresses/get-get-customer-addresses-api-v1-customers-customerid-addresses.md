# GET GetCustomerAddresses

Manages customer address information including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/customers/{customerId}/addresses
```

## Authorization

Requires authentication. Policy: **Customer.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `IEnumerable<[CustomerAddressDto](../dtos/customer-address-dto.md)>` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



