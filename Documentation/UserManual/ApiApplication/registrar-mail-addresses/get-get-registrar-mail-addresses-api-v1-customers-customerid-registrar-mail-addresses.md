# GET GetRegistrarMailAddresses

Manages registrar mail address information including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/customers/{customerId}/registrar-mail-addresses
```

## Authorization

Requires authentication. Policy: **Customer.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `customerId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | IEnumerable<[RegistrarMailAddressDto](../dtos/registrar-mail-address-dto.md)> |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




