# POST CreateToken

Creates a new token (Admin only)

## Endpoint

```
POST /api/v1/tokens
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateTokenDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `TokenDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
