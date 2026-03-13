# DELETE DeleteFtpAccount

DELETE DeleteFtpAccount

## Endpoint

```
DELETE /api/v1/hosting-accounts/{hostingAccountId}/ftp/{id}
```

## Authorization

Requires authentication. Policy: **Hosting.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `hostingAccountId` | Route | `int` |
| `id` | Route | `int` |
| `deleteFromServer` | Query | `bool` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |

[Back to API Manual index](../index.md)
