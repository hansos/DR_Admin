# PUT UpdateCountry

Update an existing country

## Endpoint

```
PUT /api/v1/countries/{id}
```

## Authorization

Requires authentication. Policy: **Geographical.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateCountryDto](../dtos/update-country-dto.md) |

[Back to API Manual index](../index.md)




