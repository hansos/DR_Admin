# POST CreateCustomerStatus

Creates a new customer status in the system

## Endpoint

```
POST /api/v1/customer-statuses
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `[CreateCustomerStatusDto](../dtos/create-customer-status-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[CustomerStatusDto](../dtos/customer-status-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



