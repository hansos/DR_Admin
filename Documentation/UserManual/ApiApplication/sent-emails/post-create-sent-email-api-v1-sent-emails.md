# POST CreateSentEmail

Creates a new sent email record

## Endpoint

```
POST /api/v1/sent-emails
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateSentEmailDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `SentEmailDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
