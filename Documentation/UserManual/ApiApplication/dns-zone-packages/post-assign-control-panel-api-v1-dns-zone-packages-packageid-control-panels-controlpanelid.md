# POST AssignControlPanel

POST AssignControlPanel

## Endpoint

```
POST /api/v1/dns-zone-packages/{packageId}/control-panels/{controlPanelId}
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
| 200 | OK | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
