# POST ResolvePayoutIntervention

POST ResolvePayoutIntervention

## Endpoint

```
POST /api/v1/vendor-payouts/resolve-intervention
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `resolveDto` | Body | `ResolvePayoutInterventionDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `VendorPayoutDto` |
| 400 | Bad Request | - |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)
