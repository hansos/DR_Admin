# POST CreateServiceType

Creates a new service type in the system

## Endpoint

```
POST /api/v1/service-types
```

## Authorization

Requires authentication. Policy: **ServiceType.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `[CreateServiceTypeDto](../dtos/create-service-type-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[ServiceTypeDto](../dtos/service-type-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



