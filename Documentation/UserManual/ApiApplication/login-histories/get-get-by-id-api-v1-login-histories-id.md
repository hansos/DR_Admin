# GET GetById

Retrieves a specific login history entry by identifier.

## Endpoint

```
GET /api/v1/login-histories/{id}
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
| 200 | OK | `[LoginHistoryDto](../dtos/login-history-dto.md)` |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



