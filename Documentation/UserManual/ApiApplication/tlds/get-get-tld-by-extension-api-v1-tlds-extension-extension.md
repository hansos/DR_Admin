# GET GetTldByExtension

Retrieves a specific TLD by its extension

## Endpoint

```
GET /api/v1/tlds/extension/{extension}
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `extension` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `TldDto` |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
