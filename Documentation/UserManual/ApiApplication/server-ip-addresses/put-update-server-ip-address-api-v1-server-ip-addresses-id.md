# PUT UpdateServerIpAddress

Updates an existing server IP address's information

## Endpoint

```
PUT /api/v1/server-ip-addresses/{id}
```

## Authorization

Requires authentication. Policy: **ServerIpAddress.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateServerIpAddressDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `ServerIpAddressDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
