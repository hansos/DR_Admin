# POST UploadRegistrarTldsCsv

Uploads a CSV file with TLDs to merge into the Tlds table and add references in RegistrarTlds table

## Endpoint

```
POST /api/v1/registrar-tlds/registrar/{registrarId}/upload-csv
```

## Authorization

Requires authentication. Policy: **Authenticated**.

## Parameters

| Name | Source | Type |
|------|--------|------|
| `registrarId` | Route | `int` |
| `uploadDto` | Form | `[UploadRegistrarTldsCsvDto](../dtos/upload-registrar-tlds-csv-dto.md)` |

## Responses

| Code | Description | Body |
|------|-------------|------|
| 200 | OK | `[ImportRegistrarTldsResponseDto](../dtos/import-registrar-tlds-response-dto.md)` |
| 400 | Bad Request | - |
| 401 | Unauthorized | - |
| 403 | Forbidden | - |
| 500 | Internal Server Error | - |

[Back to API Manual index](../index.md)



