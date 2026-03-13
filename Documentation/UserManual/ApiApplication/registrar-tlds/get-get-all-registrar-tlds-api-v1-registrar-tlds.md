# GET GetAllRegistrarTlds

Manages registrar-TLD relationships and pricing

## Endpoint

```
GET /api/v1/registrar-tlds
```

## Authorization

Requires authentication. Policy: **RegistrarTld.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |
| `isActive` | Query | `bool?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | IEnumerable<[RegistrarTldDto](../dtos/registrar-tld-dto.md)> |
| 200 | OK | [PagedResult](../dtos/paged-result.md)<[RegistrarTldDto](../dtos/registrar-tld-dto.md)> |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




