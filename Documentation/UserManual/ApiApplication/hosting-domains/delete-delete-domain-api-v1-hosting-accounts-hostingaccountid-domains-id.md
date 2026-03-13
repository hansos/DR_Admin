# DELETE DeleteDomain

Deletes a domain

## Endpoint

```
DELETE /api/v1/hosting-accounts/{hostingAccountId}/domains/{id}
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
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
