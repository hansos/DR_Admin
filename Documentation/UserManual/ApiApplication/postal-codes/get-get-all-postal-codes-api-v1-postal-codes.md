# GET GetAllPostalCodes

Manages postal codes and their geographic information

## Endpoint

```
GET /api/v1/postal-codes
```

## Authorization

Requires authentication. Policy: **Geographical.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `IEnumerable<PostalCodeDto>` |
| 200 | OK | `PagedResult<PostalCodeDto>` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
