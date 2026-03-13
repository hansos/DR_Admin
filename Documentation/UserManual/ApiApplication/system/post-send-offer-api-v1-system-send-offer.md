# POST SendOffer

Persists and marks an offer as sent while generating a server-side PDF snapshot

## Endpoint

```
POST /api/v1/system/send-offer
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
