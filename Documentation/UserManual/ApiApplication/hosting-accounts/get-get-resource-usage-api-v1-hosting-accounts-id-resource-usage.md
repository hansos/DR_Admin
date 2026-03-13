# GET GetResourceUsage

Gets resource usage statistics for a hosting account

## Endpoint

```
GET /api/v1/hosting-accounts/{id}/resource-usage
```

## Authorization

Requires authentication. Policy: **Hosting.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [ResourceUsageDto](../dtos/resource-usage-dto.md) |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




