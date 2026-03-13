# GET GetDefaultMailAddress

Retrieves the default mail address for a specific customer

## Endpoint

```
GET /api/v1/customers/{customerId}/registrar-mail-addresses/default
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
| 200 | OK | `[RegistrarMailAddressDto](../dtos/registrar-mail-address-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



