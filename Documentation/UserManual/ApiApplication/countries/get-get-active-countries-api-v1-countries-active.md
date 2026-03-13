# GET GetActiveCountries

Retrieves only active countries

## Endpoint

```
GET /api/v1/countries/active
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
| 200 | OK | `IEnumerable<CountryDto>` |
| 200 | OK | `PagedResult<CountryDto>` |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
