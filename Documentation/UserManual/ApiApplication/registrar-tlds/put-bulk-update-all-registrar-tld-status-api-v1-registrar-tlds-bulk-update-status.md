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
| `statusDto` | Body | `[BulkUpdateRegistrarTldStatusDto](../dtos/bulk-update-registrar-tld-status-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[BulkUpdateResultDto](../dtos/bulk-update-result-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



