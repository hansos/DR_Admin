# POST DownloadTldsForRegistrar

Downloads all TLDs supported by the registrar from their API and updates the database

## Endpoint

```
POST /api/v1/registrars/{registrarId}/tlds/download
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
