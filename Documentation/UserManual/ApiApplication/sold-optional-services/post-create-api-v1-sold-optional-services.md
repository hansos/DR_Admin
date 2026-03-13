# POST Create

POST Create

## Endpoint

```
POST /api/v1/sold-optional-services
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateSoldOptionalServiceDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `SoldOptionalServiceDto` |

[Back to API Manual index](../index.md)
