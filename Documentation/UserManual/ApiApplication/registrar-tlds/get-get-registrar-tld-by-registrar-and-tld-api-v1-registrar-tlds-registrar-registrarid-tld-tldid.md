# GET GetRegistrarTldByRegistrarAndTld

Retrieves a specific registrar-TLD offering by registrar and TLD combination

## Endpoint

```
GET /api/v1/registrar-tlds/registrar/{registrarId}/tld/{tldId}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarId` | Route | `int` |
| `tldId` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `RegistrarTldDto` |
| 401 | Unauthorized | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
