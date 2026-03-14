# POST QueueReply

Queues a reply email for a communication thread.

## Endpoint

```
POST /api/v1/communication-threads/{id:int}/reply
```

## Authorization

Requires authentication. Policy: **Communication.Write**.

## Request Body

`CreateCommunicationReplyDto`

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `QueueEmailResponseDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
