# PATCH MarkMessageAsRead

Marks a mailbox message as read by external message identifier.

## Endpoint

```
PATCH /api/v1/email-receiver/messages/{externalMessageId}/read
```

## Authorization

Requires authentication. Policy: **EmailReceiver.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `externalMessageId` | Route | `string` |
| `cancellationToken` | Route | `CancellationToken` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
