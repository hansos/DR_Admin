# PUT UpdateHostProvider

Updates an existing host provider's information

## Endpoint

```
PUT /api/v1/host-providers/{id}
```

## Authorization

Requires authentication. Policy: **HostProvider.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateHostProviderDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `HostProviderDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
