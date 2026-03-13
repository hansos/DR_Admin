# GET Diagnostics

Temporary diagnostics endpoint for Office365 receiver configuration and token claims.

## Endpoint

```
GET /api/v1/email-receiver/diagnostics
```

## Authorization

Requires authentication. Policy: **EmailQueue.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `folder` | Query | `string?` |
| `unreadOnly` | Query | `bool` |
| `maxItems` | Query | `int` |
| `aliasRecipient` | Query | `string?` |
| `receivedAfterUtc` | Query | `DateTime?` |
| `cancellationToken` | Route | `CancellationToken` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `EmailReceiverDiagnosticsResult` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
