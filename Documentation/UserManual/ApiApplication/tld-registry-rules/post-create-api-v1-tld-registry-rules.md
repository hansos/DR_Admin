# POST Create

Creates a new TLD registry rule.

## Endpoint

```
POST /api/v1/tld-registry-rules
```

## Authorization

Requires authentication. Policy: **Tld.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Body | `CreateTldRegistryRuleDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `TldRegistryRuleDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
