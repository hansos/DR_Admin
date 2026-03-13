# GET GetDefaultResellerCompany

Manages reseller companies including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/reseller-companies/default
```

## Authorization

Requires authentication. Policy: **ResellerCompany.Read**.

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [ResellerCompanyDto](../dtos/reseller-company-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




