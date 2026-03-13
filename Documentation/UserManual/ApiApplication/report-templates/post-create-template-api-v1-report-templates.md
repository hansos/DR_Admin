# POST CreateTemplate

Searches for report templates by name, description, or tags

## Endpoint

```
POST /api/v1/report-templates
```

## Authorization

Requires authentication. Policy: **ReportTemplate.Create**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `createDto` | Form | [CreateReportTemplateDto](../dtos/create-report-template-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 201 | Created | [ReportTemplateDto](../dtos/report-template-dto.md) |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




