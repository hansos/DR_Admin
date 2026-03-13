# GET GetById

GET GetById

## Endpoint

```
GET /api/v1/sold-optional-services/{id:int}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [SoldOptionalServiceDto](../dtos/sold-optional-service-dto.md) |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)




