# POST SendTestEmail

Provides admin-only testing endpoints.

## Endpoint

```
POST /api/v1/test/email
```

## Authorization

Requires authentication. Policy: **Policy = "Admin.Only"**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | `TestEmailRequestDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `TestEmailResultDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
