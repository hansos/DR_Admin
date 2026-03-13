# POST DownloadTldsForRegistrarFilteredList

Downloads TLDs for the registrar filtered by a list of TLD strings (provided in request body) and updates the database

## Endpoint

```
POST /api/v1/registrars/{registrarId}/tlds/download/list
```

## Authorization

Requires authentication. Policy: **Registrar.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarId` | Route | `int` |
| `tlds` | Body | `List<string>` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `object` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
