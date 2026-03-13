# GET GetAllUsers

Manages user accounts including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/users
```

## Authorization

Requires authentication. Policy: **User.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `IEnumerable<UserDto>` |
| 200 | OK | `PagedResult<UserDto>` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
