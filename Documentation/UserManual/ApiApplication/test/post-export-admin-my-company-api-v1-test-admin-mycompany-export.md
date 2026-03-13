# POST ExportAdminMyCompany

Exports the current admin user and MyCompany profile to a debug snapshot file.

## Endpoint

```
POST /api/v1/test/admin-mycompany/export
```

## Authorization

Requires authentication. Policy: **Policy = "Admin.Only"**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `fileName` | Query | `string?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `AdminUserMyCompanyExportResultDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
