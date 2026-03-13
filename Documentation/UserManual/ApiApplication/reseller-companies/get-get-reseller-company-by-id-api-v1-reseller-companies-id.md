# GET GetResellerCompanyById

Retrieves a specific reseller company by ID

## Endpoint

```
GET /api/v1/reseller-companies/{id}
```

## Authorization

Requires authentication. Policy: **Admin.Only**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [ResellerCompanyDto](../dtos/reseller-company-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




