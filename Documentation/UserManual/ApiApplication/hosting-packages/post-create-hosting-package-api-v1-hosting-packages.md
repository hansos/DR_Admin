# POST CreateHostingPackage

Creates a new hosting package in the system

## Endpoint

```
POST /api/v1/hosting-packages
```

## Authorization

Requires authentication. Policy: **HostingPackage.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateHostingPackageDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `HostingPackageDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
