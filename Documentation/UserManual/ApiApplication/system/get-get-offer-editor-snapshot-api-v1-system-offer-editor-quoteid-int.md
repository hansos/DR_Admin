# GET GetOfferEditorSnapshot

Retrieves the latest persisted offer snapshot for a quote so it can be reopened in the offer editor

## Endpoint

```
GET /api/v1/system/offer-editor/{quoteId:int}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `quoteId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
