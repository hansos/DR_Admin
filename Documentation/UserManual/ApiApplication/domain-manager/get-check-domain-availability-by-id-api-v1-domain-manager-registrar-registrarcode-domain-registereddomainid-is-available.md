# GET CheckDomainAvailabilityById

Checks if a domain is available for registration based on registered domain ID

## Endpoint

```
GET /api/v1/domain-manager/registrar/{registrarCode}/domain/{registeredDomainId}/is-available
```

## Authorization

Requires authentication. Policy: **Domain.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarCode` | Route | `string` |
| `registeredDomainId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `DomainAvailabilityResult` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
