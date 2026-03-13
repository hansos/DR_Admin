# GET GetAllDomains

Manages domains including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/registered-domains
```

## Authorization

Requires authentication. Policy: **Admin.Only**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `IEnumerable<[RegisteredDomainDto](../dtos/registered-domain-dto.md)>` |
| 200 | OK | `[PagedResult](../dtos/paged-result.md)<[RegisteredDomainDto](../dtos/registered-domain-dto.md)>` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



