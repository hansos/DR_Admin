# GET GetDefaultCustomerStatus

Manages customer statuses including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/customer-statuses/default
```

## Authorization

Requires authentication. Policy: **CustomerStatus.Read**.

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[CustomerStatusDto](../dtos/customer-status-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



