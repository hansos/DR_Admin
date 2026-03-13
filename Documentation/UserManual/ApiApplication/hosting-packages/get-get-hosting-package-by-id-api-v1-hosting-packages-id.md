# GET GetHostingPackageById

Manages hosting packages including creation, retrieval, updates, and deletion

## Endpoint

```
GET /api/v1/hosting-packages/{id}
```

## Authorization

Requires authentication. Policy: **HostingPackage.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `HostingPackageDto` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
