# PUT UpdateInvoiceLine

Update an existing invoice line

## Endpoint

```
PUT /api/v1/invoice-lines/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateInvoiceLineDto](../dtos/update-invoice-line-dto.md) |

[Back to API Manual index](../index.md)




