# POST ImportCustomerSnapshot

Handles system initialization with the first admin user

## Endpoint

```
POST /api/v1/initialization/import-customer-snapshot
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `request` | Body | `AdminUserMyCompanyImportRequestDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `AdminUserMyCompanyImportResultDto` |
| 400 | Bad Request | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
