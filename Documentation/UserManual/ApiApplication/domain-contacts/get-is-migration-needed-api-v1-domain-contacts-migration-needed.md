# GET IsMigrationNeeded

Checks if domain contact migration is needed

## Endpoint

```
GET /api/v1/domain-contacts/migration-needed
```

## Authorization

Requires authentication. Policy: **Domain.Read**.

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `bool` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
