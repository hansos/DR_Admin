# GET GetServiceTypeByName

Retrieves a specific service type by its name

## Endpoint

```
GET /api/v1/service-types/by-name/{name}
```

## Authorization

Requires authentication. Policy: **ServiceType.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `name` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [ServiceTypeDto](../dtos/service-type-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




