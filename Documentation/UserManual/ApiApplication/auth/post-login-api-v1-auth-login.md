# POST Login

Login endpoint to get JWT token. Accepts both username and email address as identification.

## Endpoint

```
POST /api/v1/auth/login
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `loginRequest` | Body | `LoginRequestDto` |

[Back to API Manual index](../index.md)
