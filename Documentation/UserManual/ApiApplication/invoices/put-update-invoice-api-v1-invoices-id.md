# PUT UpdateInvoice

Updates an existing invoice's information

## Endpoint

```
PUT /api/v1/invoices/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateInvoiceDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `InvoiceDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
