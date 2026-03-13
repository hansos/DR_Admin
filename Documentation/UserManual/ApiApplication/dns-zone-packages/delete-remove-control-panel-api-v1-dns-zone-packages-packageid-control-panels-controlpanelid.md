# DELETE RemoveControlPanel

DELETE RemoveControlPanel

## Endpoint

```
DELETE /api/v1/dns-zone-packages/{packageId}/control-panels/{controlPanelId}
```

## Authorization

Requires authentication. Policy: **DnsZonePackage.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `packageId` | Route | `int` |
| `controlPanelId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
