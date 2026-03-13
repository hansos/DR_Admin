# GET GetAllInvoices

Manages customer invoices including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/invoices
```

## Authorization

Requires authentication. Policy: **Invoice.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `IEnumerable<[InvoiceDto](../dtos/invoice-dto.md)>` |
| 200 | OK | `[PagedResult](../dtos/paged-result.md)<[InvoiceDto](../dtos/invoice-dto.md)>` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



