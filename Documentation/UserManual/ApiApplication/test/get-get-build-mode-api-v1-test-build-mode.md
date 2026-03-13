# GET GetBuildMode

Returns whether the API is running as a Debug or Release build.

## Endpoint

```
GET /api/v1/test/build-mode
```

## Authorization

Requires authentication. Policy: **Policy = "Admin.Only"**.

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |

[Back to API Manual index](../index.md)
