# GET GetServiceTypeById

Manages service types including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/service-types/{id}
```

## Authorization

Requires authentication. Policy: **ServiceType.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[ServiceTypeDto](../dtos/service-type-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



