# POST AcceptQuote

Accepts a quote using the acceptance token (public endpoint for customers)

## Endpoint

```
POST /api/v1/quotes/accept/{token}
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `token` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
