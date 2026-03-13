# GET GetPrimaryAddress

Retrieves the primary address for a specific customer

## Endpoint

```
GET /api/v1/customers/{customerId}/addresses/primary
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
| 200 | OK | `[CustomerAddressDto](../dtos/customer-address-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



