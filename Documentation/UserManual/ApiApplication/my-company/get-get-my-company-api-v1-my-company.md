# GET GetMyCompany

Manages the reseller's own company profile used in invoices, mail templates, and letterheads.

## Endpoint

```
GET /api/v1/my-company
```

## Authorization

Requires authentication. Policy: **MyCompany.Read**.

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [MyCompanyDto](../dtos/my-company-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




