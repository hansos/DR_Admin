# POST ImportRegistrarTlds

Imports TLDs for a specific registrar from CSV format form data

## Endpoint

```
POST /api/v1/registrar-tlds/registrar/{registrarId}/import
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarId` | Route | `int` |
| `importDto` | Form | [ImportRegistrarTldsDto](../dtos/import-registrar-tlds-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [ImportRegistrarTldsResponseDto](../dtos/import-registrar-tlds-response-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




