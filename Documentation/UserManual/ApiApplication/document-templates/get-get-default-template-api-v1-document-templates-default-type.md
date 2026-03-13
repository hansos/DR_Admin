# GET GetDefaultTemplate

Retrieves the default template for a specific type

## Endpoint

```
GET /api/v1/document-templates/default/{type}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `type` | Route | `DocumentTemplateType` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [DocumentTemplateDto](../dtos/document-template-dto.md) |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




