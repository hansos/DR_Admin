# GET GetRegistrarMailAddressById

Retrieves a specific registrar mail address by its unique identifier

## Endpoint

```
GET /api/v1/customers/{customerId}/registrar-mail-addresses/{id}
```

## Authorization

Requires authentication. Policy: **Customer.Read**.

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
