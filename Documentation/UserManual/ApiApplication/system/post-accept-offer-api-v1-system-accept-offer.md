# POST AcceptOffer

System-level operations including data normalization and maintenance tasks

## Endpoint

```
POST /api/v1/system/accept-offer
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `offer` | Body | `OfferDocumentDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
