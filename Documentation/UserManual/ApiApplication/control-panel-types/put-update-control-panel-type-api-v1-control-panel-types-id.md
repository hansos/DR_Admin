# PUT UpdateControlPanelType

Updates an existing control panel type's information

## Endpoint

```
PUT /api/v1/control-panel-types/{id}
```

## Authorization

Requires authentication. Policy: **ControlPanelType.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateControlPanelTypeDto](../dtos/update-control-panel-type-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [ControlPanelTypeDto](../dtos/control-panel-type-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




