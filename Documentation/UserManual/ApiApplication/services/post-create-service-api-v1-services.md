# POST CreateService

Creates a new service in the system

## Endpoint

```
POST /api/v1/services
```

## Authorization

Requires authentication. Policy: **Service.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | [CreateServiceDto](../dtos/create-service-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [ServiceDto](../dtos/service-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




