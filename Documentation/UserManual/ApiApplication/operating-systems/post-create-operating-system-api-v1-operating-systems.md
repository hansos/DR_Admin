# POST CreateOperatingSystem

Creates a new operating system in the system

## Endpoint

```
POST /api/v1/operating-systems
```

## Authorization

Requires authentication. Policy: **OperatingSystem.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateOperatingSystemDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `OperatingSystemDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
