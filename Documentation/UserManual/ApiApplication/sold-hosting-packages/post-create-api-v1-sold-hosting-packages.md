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
| `createDto` | Body | `CreateSoldHostingPackageDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `SoldHostingPackageDto` |

[Back to API Manual index](../index.md)
