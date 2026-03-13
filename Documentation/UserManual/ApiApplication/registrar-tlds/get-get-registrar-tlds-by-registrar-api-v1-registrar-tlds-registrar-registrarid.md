# GET GetRegistrarTldsByRegistrar

Retrieves only available registrar-TLD offerings for purchase

## Endpoint

```
GET /api/v1/registrar-tlds/registrar/{registrarId}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarId` | Route | `int` |
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |
| `isActive` | Query | `bool?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `IEnumerable<[RegistrarTldDto](../dtos/registrar-tld-dto.md)>` |
| 200 | OK | `[PagedResult](../dtos/paged-result.md)<[RegistrarTldDto](../dtos/registrar-tld-dto.md)>` |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



