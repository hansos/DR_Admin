# POST UploadLocalizedNamesCsv

Upload a CSV file with localized country names to merge into the Country.LocalName field     Expected CSV columns: Iso2,LocalName

## Endpoint

```
POST /api/v1/countries/upload-localized-names-csv
```

## Authorization

Requires authentication. Policy: **Geographical.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Form | DTOs.[UploadCountriesCsvDto](../dtos/upload-countries-csv-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




