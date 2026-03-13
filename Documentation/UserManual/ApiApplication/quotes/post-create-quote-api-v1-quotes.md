# POST CreateQuote

Retrieves all quotes for a specific customer

## Endpoint

```
POST /api/v1/quotes
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateQuoteDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `QuoteDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
