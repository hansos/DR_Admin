# GET GetAllTlds

Manages Top-Level Domains (TLDs) and their registrars

## Endpoint

```
GET /api/v1/tlds
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
