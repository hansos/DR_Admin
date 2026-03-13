# PUT BulkUpdateAllRegistrarTldStatus

Updates the active status of all registrar-TLD offerings in the system

## Endpoint

```
PUT /api/v1/registrar-tlds/bulk-update-status
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `statusDto` | Body | `BulkUpdateRegistrarTldStatusDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `BulkUpdateResultDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
