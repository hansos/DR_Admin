# PATCH UpdateMessageReadState

Updates the read state of a communication message.

## Endpoint

```
PATCH /api/v1/communication-threads/messages/{messageId:int}/read-state
```

## Authorization

Requires authentication. Policy: **Communication.Write**.

## Request Body

`UpdateCommunicationMessageReadStateDto`

## Parameters

| Name | Source | Type |
|------|--------|------|
| `messageId` | Route | `int` |

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
