# POST DownloadTldsForRegistrarFiltered

Manages domain registrars and their TLD offerings

## Endpoint

```
POST /api/v1/registrars/{registrarId}/tlds/download/{tld}
```

## Authorization

Requires authentication. Policy: **Registrar.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarId` | Route | `int` |
| `tld` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `object` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
