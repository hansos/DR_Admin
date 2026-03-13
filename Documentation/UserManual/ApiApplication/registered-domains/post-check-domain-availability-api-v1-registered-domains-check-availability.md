# POST CheckDomainAvailability

Checks if a domain is available for registration

## Endpoint

```
POST /api/v1/registered-domains/check-availability
```

## Authorization

Requires authentication. Policy: **Domain.CheckAvailability**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `dto` | Body | `CheckDomainAvailabilityDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `DomainAvailabilityResponseDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
