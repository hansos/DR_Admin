# PUT UpdateNameServer

Updates an existing name server

## Endpoint

```
PUT /api/v1/name-servers/{id}
```

## Authorization

Requires authentication. Policy: **NameServer.WriteOwn**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateNameServerDto](../dtos/update-name-server-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [NameServerDto](../dtos/name-server-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




