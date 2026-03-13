# POST UploadTemplate

Uploads a new document template

## Endpoint

```
POST /api/v1/document-templates/upload
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Form | `[CreateDocumentTemplateDto](../dtos/create-document-template-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | `[DocumentTemplateDto](../dtos/document-template-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



