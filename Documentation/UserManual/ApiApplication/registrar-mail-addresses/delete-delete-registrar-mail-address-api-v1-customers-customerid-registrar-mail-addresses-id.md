# DELETE DeleteRegistrarMailAddress

Deletes a registrar mail address

## Endpoint

```
DELETE /api/v1/customers/{customerId}/registrar-mail-addresses/{id}
```

## Authorization

Requires authentication. Policy: **Customer.Delete**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Route | `int` |
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
