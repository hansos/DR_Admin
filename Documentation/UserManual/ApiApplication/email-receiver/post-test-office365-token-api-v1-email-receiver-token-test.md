# POST TestOffice365Token

Provides endpoints for reading inbound emails through configured receiver plugins.

## Endpoint

```
POST /api/v1/email-receiver/token/test
```

## Authorization

Requires authentication. Policy: **EmailQueue.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `cancellationToken` | Route | `CancellationToken` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
