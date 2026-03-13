# GET GetVendorCostSummaryByInvoiceId

Retrieves vendor costs by invoice line ID

## Endpoint

```
GET /api/v1/vendor-costs/summary/invoice/{invoiceId}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `invoiceId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [VendorCostSummaryDto](../dtos/vendor-cost-summary-dto.md) |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




