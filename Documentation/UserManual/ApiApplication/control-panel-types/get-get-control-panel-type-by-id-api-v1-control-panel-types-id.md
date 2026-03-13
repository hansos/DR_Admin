# GET GetControlPanelTypeById

Manages control panel types including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/control-panel-types/{id}
```

## Authorization

Requires authentication. Policy: **ControlPanelType.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[ControlPanelTypeDto](../dtos/control-panel-type-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



