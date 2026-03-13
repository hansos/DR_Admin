# POST CreateInvoice

Creates a new invoice in the system

## Endpoint

```
POST /api/v1/invoices
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | [CreateInvoiceDto](../dtos/create-invoice-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [InvoiceDto](../dtos/invoice-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




