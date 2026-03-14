# GET GetThreads

Retrieves communication threads with optional filters.

## Endpoint

```
GET /api/v1/communication-threads
```

## Authorization

Requires authentication. Policy: **Communication.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Query | `int?` |
| `userId` | Query | `int?` |
| `relatedEntityType` | Query | `string?` |
| `relatedEntityId` | Query | `int?` |
| `status` | Query | `string?` |
| `search` | Query | `string?` |
| `maxItems` | Query | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `IEnumerable<CommunicationThreadDto>` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
