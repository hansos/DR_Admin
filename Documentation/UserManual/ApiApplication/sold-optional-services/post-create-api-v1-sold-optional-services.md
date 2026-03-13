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
| `createDto` | Body | `[CreateSoldOptionalServiceDto](../dtos/create-sold-optional-service-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[SoldOptionalServiceDto](../dtos/sold-optional-service-dto.md)` |

[Back to API Manual index](../index.md)



