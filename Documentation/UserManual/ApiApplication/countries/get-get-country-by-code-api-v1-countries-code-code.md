# GET GetCountryByCode

Retrieves a specific country by its country code

## Endpoint

```
GET /api/v1/countries/code/{code}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `code` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [CountryDto](../dtos/country-dto.md) |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




