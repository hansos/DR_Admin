# POST CreateHostProvider

Creates a new host provider in the system

## Endpoint

```
POST /api/v1/host-providers
```

## Authorization

Requires authentication. Policy: **HostProvider.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | [CreateHostProviderDto](../dtos/create-host-provider-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [HostProviderDto](../dtos/host-provider-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




