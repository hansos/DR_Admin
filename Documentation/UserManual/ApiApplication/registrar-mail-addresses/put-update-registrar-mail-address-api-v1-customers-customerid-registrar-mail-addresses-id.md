# PUT UpdateRegistrarMailAddress

Updates an existing registrar mail address

## Endpoint

```
PUT /api/v1/customers/{customerId}/registrar-mail-addresses/{id}
```

## Authorization

Requires authentication. Policy: **Customer.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Route | `int` |
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateRegistrarMailAddressDto](../dtos/update-registrar-mail-address-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [RegistrarMailAddressDto](../dtos/registrar-mail-address-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




