# GET GetServiceById

Manages services offered to customers including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/services/{id}
```

## Authorization

Requires authentication. Policy: **Service.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `ServiceDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
