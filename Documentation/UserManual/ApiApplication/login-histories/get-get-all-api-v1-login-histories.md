# GET GetAll

Controller for viewing login history entries.

## Endpoint

```
GET /api/v1/login-histories
```

## Authorization

Requires authentication. Policy: **User.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |
| `userId` | Query | `int?` |
| `isSuccessful` | Query | `bool?` |
| `from` | Query | `DateTime?` |
| `to` | Query | `DateTime?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `IEnumerable<LoginHistoryDto>` |
| 200 | OK | `PagedResult<LoginHistoryDto>` |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
