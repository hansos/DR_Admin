# PUT UpdateTemplate

Updates an existing document template's information and optionally replaces the file

## Endpoint

```
PUT /api/v1/document-templates/{id}
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Form | `UpdateDocumentTemplateDto` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `DocumentTemplateDto` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 404 | Not Found | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)
