# PUT UpdateServerControlPanel

Updates an existing server control panel's information

## Endpoint

```
PUT /api/v1/server-control-panels/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateServerControlPanelDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `ServerControlPanelDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
