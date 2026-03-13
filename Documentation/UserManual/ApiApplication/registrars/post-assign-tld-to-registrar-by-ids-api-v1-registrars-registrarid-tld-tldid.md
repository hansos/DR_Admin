# POST AssignTldToRegistrarByIds

Assigns a TLD to a registrar using their unique identifiers

## Endpoint

```
POST /api/v1/registrars/{registrarId}/tld/{tldId}
```

## Authorization

Requires authentication. Policy: **Registrar.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarId` | Route | `int` |
| `tldId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[RegistrarTldDto](../dtos/registrar-tld-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



