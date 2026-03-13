# GET GetAlternativeDomainNames

Generates alternative domain name suggestions based on active TLDs and variations of the provided name.

## Endpoint

```
GET /api/v1/domain-manager/domain/name/{domainName}/alternatives
```

## Authorization

This endpoint does not require authentication.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `domainName` | Route | `string` |
| `count` | Query | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [DomainNameAlternativesResponseDto](../dtos/domain-name-alternatives-response-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




