# GET GetActiveTlds

Retrieves only active Top-Level Domains

## Endpoint

```
GET /api/v1/tlds/active
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `IEnumerable<TldDto>` |
| 200 | OK | `PagedResult<TldDto>` |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
