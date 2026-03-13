# PUT BulkUpdateRegistrarTldStatusByTld

Updates the active status of registrar-TLD offerings for specific TLD extensions

## Endpoint

```
PUT /api/v1/registrar-tlds/bulk-update-status-by-tld
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `statusDto` | Body | `BulkUpdateRegistrarTldStatusByTldDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `BulkUpdateResultDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
