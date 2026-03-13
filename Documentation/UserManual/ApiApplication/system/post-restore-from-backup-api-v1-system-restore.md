# POST RestoreFromBackup

Restores the database from a backup file

## Endpoint

```
POST /api/v1/system/restore
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `backupFilePath` | Query | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `RestoreResultDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
