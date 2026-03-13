# GET GetAllCountries

Retrieves all countries in the system

## Endpoint

```
GET /api/v1/countries
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | IEnumerable<[CountryDto](../dtos/country-dto.md)> |
| 200 | OK | [PagedResult](../dtos/paged-result.md)<[CountryDto](../dtos/country-dto.md)> |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




