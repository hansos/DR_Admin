# GET GetAllAddressTypes

Manages address type information including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/address-types
```

## Authorization

Requires authentication. Policy: **Customer.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `pageNumber` | Query | `int?` |
| `pageSize` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `IEnumerable<AddressTypeDto>` |
| 200 | OK | `PagedResult<AddressTypeDto>` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
