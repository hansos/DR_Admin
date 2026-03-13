# GET GetById

Manages TLD registry policy rules.

## Endpoint

```
GET /api/v1/tld-registry-rules/{id:int}
```

## Authorization

Requires authentication. Policy: **Tld.Read**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[TldRegistryRuleDto](../dtos/tld-registry-rule-dto.md)` |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



