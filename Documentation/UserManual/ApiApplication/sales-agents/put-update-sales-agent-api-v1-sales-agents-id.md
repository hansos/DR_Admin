# PUT UpdateSalesAgent

Updates an existing sales agent

## Endpoint

```
PUT /api/v1/sales-agents/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateSalesAgentDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `SalesAgentDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
