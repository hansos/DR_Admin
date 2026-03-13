# PUT UpdateRegistrarTld

Updates an existing registrar-TLD offering with new pricing information

## Endpoint

```
PUT /api/v1/registrar-tlds/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateRegistrarTldDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `RegistrarTldDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
