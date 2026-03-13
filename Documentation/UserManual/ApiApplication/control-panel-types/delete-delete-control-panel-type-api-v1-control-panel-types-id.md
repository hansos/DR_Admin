# DELETE DeleteControlPanelType

Deletes a control panel type from the system

## Endpoint

```
DELETE /api/v1/control-panel-types/{id}
```

## Authorization

Requires authentication. Policy: **ControlPanelType.Delete**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
