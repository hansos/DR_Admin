# POST NormalizeAllRecords

Normalizes all records in the database by updating normalized fields for exact searches

## Endpoint

```
POST /api/v1/system/normalize-all-records
```

## Authorization

Requires authentication. Policy: **Admin.Only**.

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `NormalizationResultDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
