# GET GetServerIpAddressById

Manages server IP addresses including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/server-ip-addresses/{id}
```

## Authorization

Requires authentication. Policy: **ServerIpAddress.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[ServerIpAddressDto](../dtos/server-ip-address-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



