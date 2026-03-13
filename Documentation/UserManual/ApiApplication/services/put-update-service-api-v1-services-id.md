# PUT UpdateService

Updates an existing service's information

## Endpoint

```
PUT /api/v1/services/{id}
```

## Authorization

Requires authentication. Policy: **Service.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateServiceDto](../dtos/update-service-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [ServiceDto](../dtos/service-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




