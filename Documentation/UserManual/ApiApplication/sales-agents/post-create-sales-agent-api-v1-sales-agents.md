# POST CreateSalesAgent

Creates a new sales agent

## Endpoint

```
POST /api/v1/sales-agents
```

## Authorization

Requires authentication. Policy: **SalesAgent.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `[CreateSalesAgentDto](../dtos/create-sales-agent-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[SalesAgentDto](../dtos/sales-agent-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



