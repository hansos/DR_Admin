# GET GetSentEmailByMessageId

Manages sent email records including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/sent-emails/by-message-id/{messageId}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `messageId` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[SentEmailDto](../dtos/sent-email-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



