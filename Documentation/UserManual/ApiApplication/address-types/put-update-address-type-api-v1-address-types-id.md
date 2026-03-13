# PUT UpdateAddressType

Updates an existing address type

## Endpoint

```
PUT /api/v1/address-types/{id}
```

## Authorization

Requires authentication. Policy: **Customer.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateAddressTypeDto](../dtos/update-address-type-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [AddressTypeDto](../dtos/address-type-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




