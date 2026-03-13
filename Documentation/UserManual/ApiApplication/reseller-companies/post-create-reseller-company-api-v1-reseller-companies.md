# POST CreateResellerCompany

Creates a new reseller company

## Endpoint

```
POST /api/v1/reseller-companies
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateResellerCompanyDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `ResellerCompanyDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
