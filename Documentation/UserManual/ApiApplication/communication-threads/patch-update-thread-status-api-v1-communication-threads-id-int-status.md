# PATCH UpdateThreadStatus

Updates the status of a communication thread.

## Endpoint

```
PATCH /api/v1/communication-threads/{id:int}/status
```

## Authorization

Requires authentication. Policy: **Communication.Write**.

## Request Body

`UpdateCommunicationThreadStatusDto`

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

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
