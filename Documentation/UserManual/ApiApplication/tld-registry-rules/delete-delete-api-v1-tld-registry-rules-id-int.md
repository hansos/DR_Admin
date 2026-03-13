# DELETE Delete

Deletes a TLD registry rule.

## Endpoint

```
DELETE /api/v1/tld-registry-rules/{id:int}
```

## Authorization

Requires authentication. Policy: **Tld.Delete**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 204 | No Content | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
