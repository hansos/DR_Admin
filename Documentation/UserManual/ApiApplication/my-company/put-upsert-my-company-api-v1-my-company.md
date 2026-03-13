# PUT UpsertMyCompany

Creates or updates the company profile.

## Endpoint

```
PUT /api/v1/my-company
```

## Authorization

Requires authentication. Policy: **MyCompany.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `UpsertMyCompanyDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `MyCompanyDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
