# GET GetRegistrarTldById

Retrieves all registrars offering a specific TLD

## Endpoint

```
GET /api/v1/registrar-tlds/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[RegistrarTldDto](../dtos/registrar-tld-dto.md)` |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



