# GET GetById

Provides read access to registered domain history entries.

## Endpoint

```
GET /api/v1/registered-domain-histories/{id:int}
```

## Authorization

Requires authentication. Policy: **DomainHistory.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [RegisteredDomainHistoryDto](../dtos/registered-domain-history-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




