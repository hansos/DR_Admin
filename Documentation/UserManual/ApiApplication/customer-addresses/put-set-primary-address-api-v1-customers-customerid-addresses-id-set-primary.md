# PUT SetPrimaryAddress

Sets a customer address as the primary address

## Endpoint

```
PUT /api/v1/customers/{customerId}/addresses/{id}/set-primary
```

## Authorization

Requires authentication. Policy: **Customer.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Route | `int` |
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [CustomerAddressDto](../dtos/customer-address-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




