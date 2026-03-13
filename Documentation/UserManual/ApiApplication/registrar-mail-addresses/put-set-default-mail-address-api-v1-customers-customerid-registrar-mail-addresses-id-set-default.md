# PUT SetDefaultMailAddress

Sets a registrar mail address as the default for the customer

## Endpoint

```
PUT /api/v1/customers/{customerId}/registrar-mail-addresses/{id}/set-default
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
| 200 | OK | `RegistrarMailAddressDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
