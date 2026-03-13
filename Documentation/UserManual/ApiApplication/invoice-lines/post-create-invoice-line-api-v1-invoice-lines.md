# POST CreateInvoiceLine

Retrieves all line items for a specific invoice

## Endpoint

```
POST /api/v1/invoice-lines
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | [CreateInvoiceLineDto](../dtos/create-invoice-line-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [InvoiceLineDto](../dtos/invoice-line-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




