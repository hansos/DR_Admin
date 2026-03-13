# PUT UpdateUser

Updates an existing user's information

## Endpoint

```
PUT /api/v1/users/{id}
```

## Authorization

Requires authentication. Policy: **User.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `[UpdateUserDto](../dtos/update-user-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[UserDto](../dtos/user-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



