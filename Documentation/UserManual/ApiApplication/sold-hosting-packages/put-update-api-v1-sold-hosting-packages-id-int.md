# PUT Update

PUT Update

## Endpoint

```
PUT /api/v1/sold-hosting-packages/{id:int}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateSoldHostingPackageDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `SoldHostingPackageDto` |
| 404 | Not Found | - |

[Back to API Manual index](../index.md)
