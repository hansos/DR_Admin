# GET CheckDomainAvailabilityByName

Checks if a domain is available for registration based on domain name

## Endpoint

```
GET /api/v1/domain-manager/registrar/default/domain/{domainName}/is-available
```

## Authorization

Requires authentication. Policy: **Domain.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
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
