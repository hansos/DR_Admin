# GET GetAllContactPersons

Manages contact persons for customers including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/contact-persons
```

## Authorization

Requires authentication. Policy: **Customer.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |
| `customerId` | Query | `int?` |
| `search` | Query | `string?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `IEnumerable<ContactPersonDto>` |
| 200 | OK | `PagedResult<ContactPersonDto>` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
