# GET GetHostProviderById

Manages host providers including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/host-providers/{id}
```

## Authorization

Requires authentication. Policy: **HostProvider.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [HostProviderDto](../dtos/host-provider-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




