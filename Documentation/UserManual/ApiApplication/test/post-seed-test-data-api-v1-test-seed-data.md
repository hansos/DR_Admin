# POST SeedTestData

Seeds test catalog data into selected tables when those tables are empty.

## Endpoint

```
POST /api/v1/test/seed-data
```

## Authorization

Requires authentication. Policy: **Policy = "Admin.Only"**.

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `SeedTestDataResultDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
