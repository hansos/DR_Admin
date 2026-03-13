# POST SetCountriesActiveByCodes

Set active flag for a selection of countries by codes (comma separated or JSON array)

## Endpoint

```
POST /api/v1/countries/set-active-by-codes
```

## Authorization

Requires authentication. Policy: **Geographical.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `codes` | Query | `string?` |
| `isActive` | Query | `bool` |
| `bodyCodes` | Body | `IEnumerable<string>?` |

[Back to API Manual index](../index.md)
