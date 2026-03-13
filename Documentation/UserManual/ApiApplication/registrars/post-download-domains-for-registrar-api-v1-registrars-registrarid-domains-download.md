# POST DownloadDomainsForRegistrar

Downloads all domains registered with a specific registrar and syncs them to the database

## Endpoint

```
POST /api/v1/registrars/{registrarId}/domains/download
```

## Authorization

Requires authentication. Policy: **Registrar.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `object` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
