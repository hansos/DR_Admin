# POST RetryEmail

Queues an existing failed or pending email for immediate retry.

## Endpoint

```
POST /api/v1/email-queue/retry/{id}
```

## Authorization

Requires authentication. Policy: **EmailQueue.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
