# POST CreateCountry

Creates a new country in the system

## Endpoint

```
POST /api/v1/countries
```

## Authorization

Requires authentication. Policy: **Geographical.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `[CreateCountryDto](../dtos/create-country-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[CountryDto](../dtos/country-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



