# GET GetDomainByName

Retrieves a specific domain by name

## Endpoint

```
GET /api/v1/registered-domains/name/{name}
```

## Authorization

Requires authentication. Policy: **Domain.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `name` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `RegisteredDomainDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
