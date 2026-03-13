# POST CreateRegistrarTld

Creates a new registrar-TLD offering with pricing information

## Endpoint

```
POST /api/v1/registrar-tlds
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateRegistrarTldDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `RegistrarTldDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
