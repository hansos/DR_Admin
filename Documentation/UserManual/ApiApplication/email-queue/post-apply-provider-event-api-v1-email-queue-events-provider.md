# POST ApplyProviderEvent

Applies a provider delivery event to email queue and communication status.

## Endpoint

```
POST /api/v1/email-queue/events/provider
```

## Authorization

Requires authentication. Policy: **EmailQueue.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `providerEventDto` | Body | [EmailProviderEventDto](../dtos/email-provider-event-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 400 | Bad Request | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
