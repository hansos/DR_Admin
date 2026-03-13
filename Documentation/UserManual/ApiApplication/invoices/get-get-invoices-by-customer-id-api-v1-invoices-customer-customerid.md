# GET GetInvoicesByCustomerId

Retrieves all invoices for a specific customer

## Endpoint

```
GET /api/v1/invoices/customer/{customerId}
```

## Authorization

Requires authentication. Policy: **Invoice.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Route | `int` |
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | IEnumerable<[InvoiceDto](../dtos/invoice-dto.md)> |
| 200 | OK | [PagedResult](../dtos/paged-result.md)<[InvoiceDto](../dtos/invoice-dto.md)> |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




