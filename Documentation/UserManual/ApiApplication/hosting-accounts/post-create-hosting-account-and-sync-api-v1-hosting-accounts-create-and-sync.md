# POST CreateHostingAccountAndSync

Creates a new hosting account and synchronizes it to the server

## Endpoint

```
POST /api/v1/hosting-accounts/create-and-sync
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `HostingAccountCreateDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `HostingAccountDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
