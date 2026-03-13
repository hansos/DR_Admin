# GET CheckDomainAvailabilityByName

Checks if a domain is available for registration based on domain name

## Endpoint

```
GET /api/v1/domain-manager/registrar/{registrarCode}/domain/name/{domainName}/is-available
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarCode` | Route | `string` |
| `domainName` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `DomainAvailabilityResult` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
