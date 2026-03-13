# GET GetDomainPricing

Gets pricing information for a specific TLD

## Endpoint

```
GET /api/v1/registered-domains/pricing/{tld}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `tld` | Route | `string` |
| `registrarId` | Query | `int?` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `DomainPricingDto` |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
