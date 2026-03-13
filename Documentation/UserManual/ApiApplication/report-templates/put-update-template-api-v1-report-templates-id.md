# PUT UpdateTemplate

Updates an existing report template

## Endpoint

```
PUT /api/v1/report-templates/{id}
```

## Authorization

Requires authentication. Policy: **ReportTemplate.Update**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `id` | Route | `int` |
| `updateDto` | Form | [UpdateReportTemplateDto](../dtos/update-report-template-dto.md) |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | [ReportTemplateDto](../dtos/report-template-dto.md) |
| 400 | Bad Request | - |
| 404 | Not Found | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)




