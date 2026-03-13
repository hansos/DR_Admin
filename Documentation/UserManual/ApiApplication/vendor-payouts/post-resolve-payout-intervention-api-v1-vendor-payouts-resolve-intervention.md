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
| `resolveDto` | Body | `[ResolvePayoutInterventionDto](../dtos/resolve-payout-intervention-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[VendorPayoutDto](../dtos/vendor-payout-dto.md)` |
| 400 | Bad Request | - |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)



