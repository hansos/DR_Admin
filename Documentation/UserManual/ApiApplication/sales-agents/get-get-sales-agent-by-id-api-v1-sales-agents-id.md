# GET GetSalesAgentById

Manages sales agents including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/sales-agents/{id}
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
| 200 | OK | `[SalesAgentDto](../dtos/sales-agent-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



