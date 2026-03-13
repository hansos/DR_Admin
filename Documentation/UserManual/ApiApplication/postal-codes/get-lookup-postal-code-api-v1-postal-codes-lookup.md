# GET LookupPostalCode

Lookup a postal code by country code and postal code combination

## Endpoint

```
GET /api/v1/postal-codes/lookup
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `countryCode` | Query | `string` |
| `postalCode` | Query | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `PostalCodeDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
