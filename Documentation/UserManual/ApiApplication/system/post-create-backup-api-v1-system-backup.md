# POST CreateBackup

Creates a backup of the database

## Endpoint

```
POST /api/v1/system/backup
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `backupFileName` | Query | `string?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `BackupResultDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
