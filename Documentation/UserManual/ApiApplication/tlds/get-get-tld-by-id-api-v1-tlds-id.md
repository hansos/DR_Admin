# GET GetTldById

Retrieves a specific TLD by its unique identifier

## Endpoint

```
GET /api/v1/tlds/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `TldDto` |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
