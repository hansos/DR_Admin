# GET GetInvoiceByNumber

Retrieves a specific invoice by its invoice number

## Endpoint

```
GET /api/v1/invoices/number/{invoiceNumber}
```

## Authorization

Requires authentication. Policy: **Invoice.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `invoiceNumber` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[InvoiceDto](../dtos/invoice-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



