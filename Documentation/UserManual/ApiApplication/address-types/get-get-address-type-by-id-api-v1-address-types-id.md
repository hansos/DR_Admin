# GET GetAddressTypeById

Retrieves a specific address type by its unique identifier

## Endpoint

```
GET /api/v1/address-types/{id}
```

## Authorization

Requires authentication. Policy: **Customer.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [AddressTypeDto](../dtos/address-type-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




