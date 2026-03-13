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
| `dto` | Body | [CheckDomainAvailabilityDto](../dtos/check-domain-availability-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [DomainAvailabilityResponseDto](../dtos/domain-availability-response-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




