# GET GetEmailStatus

Gets the status of a queued email

## Endpoint

```
GET /api/v1/email-queue/status/{id}
```

## Authorization

Requires authentication. Policy: **EmailQueue.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `SentEmailDto` |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
