# POST CreateNameServer

Retrieves all name servers for a specific domain

## Endpoint

```
POST /api/v1/name-servers
```

## Authorization

Requires authentication. Policy: **NameServer.WriteOwn**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateNameServerDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `NameServerDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
