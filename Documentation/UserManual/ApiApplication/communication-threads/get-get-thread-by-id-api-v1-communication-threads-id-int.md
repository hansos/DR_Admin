# GET GetThreadById

Retrieves a communication thread by identifier including participants and messages.

## Endpoint

```
GET /api/v1/communication-threads/{id:int}
```

## Authorization

Requires authentication. Policy: **Communication.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `CommunicationThreadDetailsDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
