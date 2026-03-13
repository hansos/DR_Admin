# DELETE RemoveServer

DELETE RemoveServer

## Endpoint

```
DELETE /api/v1/dns-zone-packages/{packageId}/servers/{serverId}
```

## Authorization

Requires authentication. Policy: **DnsZonePackage.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `packageId` | Route | `int` |
| `serverId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
