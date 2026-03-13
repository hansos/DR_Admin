# GET GetQuoteById

Manages sales quotes and proposals

## Endpoint

```
GET /api/v1/quotes/{id}
```

## Authorization

Requires authentication. Policy: **Quote.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `QuoteDto` |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
