# POST MigrateDomainContactsToContactPersons

Migrates domain contacts to the normalized ContactPerson and DomainContactAssignment tables

## Endpoint

```
POST /api/v1/domain-contacts/migrate-to-contact-persons
```

## Authorization

Requires authentication. Policy: **Domain.Write**.

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `MigrationResult` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
