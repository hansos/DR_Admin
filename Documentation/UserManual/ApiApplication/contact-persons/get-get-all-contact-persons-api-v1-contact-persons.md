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
| 200 | OK | IEnumerable<[ContactPersonDto](../dtos/contact-person-dto.md)> |
| 200 | OK | [PagedResult](../dtos/paged-result.md)<[ContactPersonDto](../dtos/contact-person-dto.md)> |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




