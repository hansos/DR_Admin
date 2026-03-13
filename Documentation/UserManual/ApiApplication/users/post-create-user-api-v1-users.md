# POST CreateUser

Creates a new user in the system

## Endpoint

```
POST /api/v1/users
```

## Authorization

Requires authentication. Policy: **User.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `[CreateUserDto](../dtos/create-user-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[UserDto](../dtos/user-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



