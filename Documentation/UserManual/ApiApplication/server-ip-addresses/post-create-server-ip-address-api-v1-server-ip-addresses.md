# POST CreateServerIpAddress

Creates a new server IP address in the system

## Endpoint

```
POST /api/v1/server-ip-addresses
```

## Authorization

Requires authentication. Policy: **ServerIpAddress.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateServerIpAddressDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `ServerIpAddressDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
