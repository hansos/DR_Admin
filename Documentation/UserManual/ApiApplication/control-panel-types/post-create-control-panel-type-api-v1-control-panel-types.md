# POST CreateControlPanelType

Creates a new control panel type in the system

## Endpoint

```
POST /api/v1/control-panel-types
```

## Authorization

Requires authentication. Policy: **ControlPanelType.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | [CreateControlPanelTypeDto](../dtos/create-control-panel-type-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [ControlPanelTypeDto](../dtos/control-panel-type-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




