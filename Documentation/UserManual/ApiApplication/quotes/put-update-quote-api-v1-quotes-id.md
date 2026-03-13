# PUT UpdateQuote

Updates an existing quote

## Endpoint

```
PUT /api/v1/quotes/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `[UpdateQuoteDto](../dtos/update-quote-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[QuoteDto](../dtos/quote-dto.md)` |
| 400 | Bad Request | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



