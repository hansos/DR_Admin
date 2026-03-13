# POST AssignTldToRegistrarByDto

Assigns a TLD to a registrar using TLD details

## Endpoint

```
POST /api/v1/registrars/{registrarId}/tld
```

## Authorization

Requires authentication. Policy: **Registrar.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarId` | Route | `int` |
| `tldDto` | Body | `TldDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `RegistrarTldDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
