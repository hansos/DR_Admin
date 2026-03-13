# PUT UpdateToken

Updates an existing token's information

## Endpoint

```
PUT /api/v1/tokens/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateTokenDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `TokenDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
