# GET CheckDomainAvailability

Checks if a domain is available for registration using a specific registrar

## Endpoint

```
GET /api/v1/registrars/{registrarId}/isavailable/{domainName}
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarId` | Route | `int` |
| `domainName` | Route | `string` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `DomainRegistrationLib.Models.DomainAvailabilityResult` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
