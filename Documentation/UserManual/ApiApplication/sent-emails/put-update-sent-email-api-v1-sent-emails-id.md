# PUT UpdateSentEmail

Updates an existing sent email record

## Endpoint

```
PUT /api/v1/sent-emails/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateSentEmailDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `SentEmailDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
