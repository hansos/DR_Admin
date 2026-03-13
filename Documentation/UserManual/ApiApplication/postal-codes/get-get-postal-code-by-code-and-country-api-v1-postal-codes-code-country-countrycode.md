# GET GetPostalCodeByCodeAndCountry

Retrieves a specific postal code by code and country

## Endpoint

```
GET /api/v1/postal-codes/{code}/country/{countryCode}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `code` | Route | `string` |
| `countryCode` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [PostalCodeDto](../dtos/postal-code-dto.md) |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




