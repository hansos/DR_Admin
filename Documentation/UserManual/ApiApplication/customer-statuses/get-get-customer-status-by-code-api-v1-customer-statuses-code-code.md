# GET GetCustomerStatusByCode

Retrieves a specific customer status by its code

## Endpoint

```
GET /api/v1/customer-statuses/code/{code}
```

## Authorization

Requires authentication. Policy: **CustomerStatus.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `code` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `CustomerStatusDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
