# GET GetNameServerById

Retrieves a specific name server by its unique identifier

## Endpoint

```
GET /api/v1/name-servers/{id}
```

## Authorization

Requires authentication. Policy: **NameServer.ReadOwn**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[NameServerDto](../dtos/name-server-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



