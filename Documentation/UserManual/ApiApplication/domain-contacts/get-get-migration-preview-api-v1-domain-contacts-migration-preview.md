# GET GetMigrationPreview

Gets a preview of what the migration would do without performing it

## Endpoint

```
GET /api/v1/domain-contacts/migration-preview
```

## Authorization

Requires authentication. Policy: **Domain.Read**.

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `MigrationPreview` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
