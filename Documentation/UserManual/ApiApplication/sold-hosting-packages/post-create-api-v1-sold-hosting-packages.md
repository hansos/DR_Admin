# POST Create

POST Create

## Endpoint

```
POST /api/v1/sold-hosting-packages
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `[CreateSoldHostingPackageDto](../dtos/create-sold-hosting-package-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[SoldHostingPackageDto](../dtos/sold-hosting-package-dto.md)` |

[Back to API Manual index](../index.md)



