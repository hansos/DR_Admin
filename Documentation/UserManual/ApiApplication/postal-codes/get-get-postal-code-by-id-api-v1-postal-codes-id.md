# GET GetPostalCodeById

Retrieves all postal codes for a specific country

## Endpoint

```
GET /api/v1/postal-codes/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[PostalCodeDto](../dtos/postal-code-dto.md)` |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



