# PUT UpdateOperatingSystem

Updates an existing operating system's information

## Endpoint

```
PUT /api/v1/operating-systems/{id}
```

## Authorization

Requires authentication. Policy: **OperatingSystem.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateOperatingSystemDto](../dtos/update-operating-system-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [OperatingSystemDto](../dtos/operating-system-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




