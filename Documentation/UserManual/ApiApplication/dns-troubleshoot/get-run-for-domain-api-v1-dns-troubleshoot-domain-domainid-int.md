# GET RunForDomain

Provides DNS troubleshooting checks for domains.

## Endpoint

```
GET /api/v1/dns-troubleshoot/domain/{domainId:int}
```

## Authorization

Requires authentication. Policy: **DnsRecord.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `domainId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `DnsTroubleshootReportDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
