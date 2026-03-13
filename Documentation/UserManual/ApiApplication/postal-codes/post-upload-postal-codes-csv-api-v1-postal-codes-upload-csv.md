# POST UploadPostalCodesCsv

Upload a CSV file with postal codes to merge into the PostalCodes table for a specific country.     Expected CSV format: PostalCode,City,State,Region,District,Latitude,Longitude,IsActive (minimum: PostalCode,City)

## Endpoint

```
POST /api/v1/postal-codes/upload-csv
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Form | `UploadPostalCodesCsvDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `ImportPostalCodesResultDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
