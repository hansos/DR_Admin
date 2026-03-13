# GET GetAllDomainContacts

Manages domain contact persons for domain registrations including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/domain-contacts
```

## Authorization

Requires authentication. Policy: **Domain.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | IEnumerable<[DomainContactDto](../dtos/domain-contact-dto.md)> |
| 200 | OK | [PagedResult](../dtos/paged-result.md)<[DomainContactDto](../dtos/domain-contact-dto.md)> |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




