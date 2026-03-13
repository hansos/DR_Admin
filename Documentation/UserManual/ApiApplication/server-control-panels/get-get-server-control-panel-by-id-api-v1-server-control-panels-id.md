# GET GetServerControlPanelById

Manages server control panels including creation, retrieval, updates, deletion, and connection testing

## Endpoint

```
GET /api/v1/server-control-panels/{id}
```

## Authorization

Requires authentication. Policy: **ServerControlPanel.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `ServerControlPanelDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
