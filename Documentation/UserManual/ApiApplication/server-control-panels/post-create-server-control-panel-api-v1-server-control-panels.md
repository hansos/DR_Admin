# POST CreateServerControlPanel

Creates a new server control panel in the system

## Endpoint

```
POST /api/v1/server-control-panels
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | [CreateServerControlPanelDto](../dtos/create-server-control-panel-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [ServerControlPanelDto](../dtos/server-control-panel-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




