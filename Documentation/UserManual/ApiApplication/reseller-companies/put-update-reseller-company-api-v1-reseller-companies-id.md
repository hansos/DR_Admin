# PUT UpdateResellerCompany

Updates an existing reseller company

## Endpoint

```
PUT /api/v1/reseller-companies/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateResellerCompanyDto](../dtos/update-reseller-company-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [ResellerCompanyDto](../dtos/reseller-company-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




