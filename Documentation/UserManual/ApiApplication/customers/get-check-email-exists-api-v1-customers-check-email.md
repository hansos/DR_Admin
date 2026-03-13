# GET CheckEmailExists

Searches customers by a free-text query across name, customer name, email, phone,     reference number, customer number, and associated contact person details

## Endpoint

```
GET /api/v1/customers/check-email
```

## Authorization

Requires authentication. Policy: **Customer.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `email` | Query | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `EmailExistsDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
