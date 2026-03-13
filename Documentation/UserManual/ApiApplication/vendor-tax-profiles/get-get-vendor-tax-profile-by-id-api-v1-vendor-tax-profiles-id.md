# GET GetVendorTaxProfileById

Manages vendor tax profiles

## Endpoint

```
GET /api/v1/vendor-tax-profiles/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `VendorTaxProfileDto` |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)
