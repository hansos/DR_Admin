# PUT UpdateHostingPackage

Updates an existing hosting package's information

## Endpoint

```
PUT /api/v1/hosting-packages/{id}
```

## Authorization

Requires authentication. Policy: **HostingPackage.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | `UpdateHostingPackageDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `HostingPackageDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
