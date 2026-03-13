# GET GetUserById

Retrieves a specific user by their unique identifier

## Endpoint

```
GET /api/v1/users/{id}
```

## Authorization

Requires authentication. Policy: **User.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [UserDto](../dtos/user-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




