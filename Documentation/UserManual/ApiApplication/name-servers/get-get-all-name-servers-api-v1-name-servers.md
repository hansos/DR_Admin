# GET GetAllNameServers

Manages name servers for domains

## Endpoint

```
GET /api/v1/name-servers
```

## Authorization

Requires authentication. Policy: **NameServer.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | IEnumerable<[NameServerDto](../dtos/name-server-dto.md)> |
| 200 | OK | [PagedResult](../dtos/paged-result.md)<[NameServerDto](../dtos/name-server-dto.md)> |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




