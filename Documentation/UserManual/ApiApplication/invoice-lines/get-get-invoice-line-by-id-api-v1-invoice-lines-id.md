# GET GetInvoiceLineById

Retrieves a specific invoice line by its unique identifier

## Endpoint

```
GET /api/v1/invoice-lines/{id}
```

## Authorization

Requires authentication. Policy: **InvoiceLine.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[InvoiceLineDto](../dtos/invoice-line-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



