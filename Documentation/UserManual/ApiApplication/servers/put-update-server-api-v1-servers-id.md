# PUT UpdateServer

Updates an existing server's information

## Endpoint

```
PUT /api/v1/servers/{id}
```

## Authorization

Requires authentication. Policy: **Server.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateServerDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `ServerDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
