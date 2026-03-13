# DELETE DeleteOperatingSystem

Deletes an operating system from the system

## Endpoint

```
DELETE /api/v1/operating-systems/{id}
```

## Authorization

Requires authentication. Policy: **OperatingSystem.Delete**.

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
