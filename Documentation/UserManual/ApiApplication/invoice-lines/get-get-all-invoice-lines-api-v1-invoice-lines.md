# GET GetAllInvoiceLines

Manages invoice line items representing individual charges on invoices

## Endpoint

```
GET /api/v1/invoice-lines
```

## Authorization

Requires authentication. Policy: **InvoiceLine.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `IEnumerable<InvoiceLineDto>` |
| 200 | OK | `PagedResult<InvoiceLineDto>` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
