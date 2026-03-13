# POST QueueEmail

Manages email queue operations for sending emails asynchronously

## Endpoint

```
POST /api/v1/email-queue/queue
```

## Authorization

Requires authentication. Policy: **EmailQueue.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `queueEmailDto` | Body | `QueueEmailDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `QueueEmailResponseDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
