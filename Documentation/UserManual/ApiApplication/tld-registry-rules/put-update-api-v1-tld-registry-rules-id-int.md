# PUT Update

Updates an existing TLD registry rule.

## Endpoint

```
PUT /api/v1/tld-registry-rules/{id:int}
```

## Authorization

Requires authentication. Policy: **Tld.Write**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Body | [UpdateTldRegistryRuleDto](../dtos/update-tld-registry-rule-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [TldRegistryRuleDto](../dtos/tld-registry-rule-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




