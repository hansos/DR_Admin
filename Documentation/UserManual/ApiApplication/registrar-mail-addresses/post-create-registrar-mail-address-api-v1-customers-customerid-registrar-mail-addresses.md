# POST CreateRegistrarMailAddress

Creates a new registrar mail address

## Endpoint

```
POST /api/v1/customers/{customerId}/registrar-mail-addresses
```

## Authorization

Requires authentication. Policy: **Customer.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Route | `int` |
| `createDto` | Body | `[CreateRegistrarMailAddressDto](../dtos/create-registrar-mail-address-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[RegistrarMailAddressDto](../dtos/registrar-mail-address-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



