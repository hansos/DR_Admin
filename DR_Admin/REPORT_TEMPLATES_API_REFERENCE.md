# Report Templates API Quick Reference

## Base URL
```
/api/v1/ReportTemplates
```

## Authentication
All endpoints require authentication via JWT Bearer token.

## Endpoints

### 1. Get All Templates
```http
GET /api/v1/ReportTemplates
Authorization: Bearer {token}
```
**Authorization**: Admin, Support  
**Returns**: Array of ReportTemplateDto

---

### 2. Get Active Templates
```http
GET /api/v1/ReportTemplates/active
Authorization: Bearer {token}
```
**Authorization**: Admin, Support  
**Returns**: Array of active ReportTemplateDto

---

### 3. Get Templates by Type
```http
GET /api/v1/ReportTemplates/type/{type}
Authorization: Bearer {token}
```
**Authorization**: Admin, Support  
**Parameters**:
- `type` (path): ReportTemplateType enum value (0-7 or 99)
  - 0 = Invoice
  - 1 = CustomerList
  - 2 = SalesReport
  - 3 = DomainReport
  - 4 = HostingReport
  - 5 = FinancialReport
  - 6 = SubscriptionReport
  - 7 = OrderReport
  - 99 = Custom

**Returns**: Array of ReportTemplateDto

---

### 4. Get Templates by Engine
```http
GET /api/v1/ReportTemplates/engine/{engine}
Authorization: Bearer {token}
```
**Authorization**: Admin, Support  
**Parameters**:
- `engine` (path): Reporting engine name (e.g., "FastReport", "RDLC")

**Returns**: Array of ReportTemplateDto

---

### 5. Get Template by ID
```http
GET /api/v1/ReportTemplates/{id}
Authorization: Bearer {token}
```
**Authorization**: Admin, Support  
**Parameters**:
- `id` (path): Template ID

**Returns**: ReportTemplateDto or 404

---

### 6. Get Default Template
```http
GET /api/v1/ReportTemplates/default/{type}
Authorization: Bearer {token}
```
**Authorization**: Admin, Support  
**Parameters**:
- `type` (path): ReportTemplateType enum value

**Returns**: ReportTemplateDto or 404

---

### 7. Search Templates
```http
GET /api/v1/ReportTemplates/search?searchTerm={term}
Authorization: Bearer {token}
```
**Authorization**: Admin, Support  
**Query Parameters**:
- `searchTerm` (required): Search term to match against name, description, or tags

**Returns**: Array of matching ReportTemplateDto

---

### 8. Download Template File
```http
GET /api/v1/ReportTemplates/{id}/download
Authorization: Bearer {token}
```
**Authorization**: Admin, Support  
**Parameters**:
- `id` (path): Template ID

**Returns**: File download with original filename and MIME type

---

### 9. Create Template
```http
POST /api/v1/ReportTemplates
Authorization: Bearer {token}
Content-Type: multipart/form-data
```
**Authorization**: Admin only  
**Form Data**:
- `Name` (required): Template name (max 200 chars)
- `Description` (required): Template description (max 1000 chars)
- `TemplateType` (required): ReportTemplateType enum value (0-7 or 99)
- `ReportEngine` (optional): Default "FastReport" (max 50 chars)
- `File` (required): Template file (.frx, .rdlc, or .rpt, max 50MB)
- `IsActive` (optional): Boolean, default true
- `IsDefault` (optional): Boolean, default false
- `DataSourceInfo` (optional): JSON string with data source information
- `Version` (optional): Version string, default "1.0"
- `Tags` (optional): Comma-separated tags (max 500 chars)
- `DefaultExportFormat` (optional): Default "PDF" (max 20 chars)

**Returns**: Created ReportTemplateDto (201) or validation error (400)

**Example using curl**:
```bash
curl -X POST "https://api.example.com/api/v1/ReportTemplates" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "Name=Monthly Invoice Template" \
  -F "Description=Template for monthly recurring invoices" \
  -F "TemplateType=0" \
  -F "ReportEngine=FastReport" \
  -F "File=@/path/to/invoice.frx" \
  -F "IsActive=true" \
  -F "IsDefault=true" \
  -F "DefaultExportFormat=PDF" \
  -F "Tags=invoice,monthly,recurring"
```

---

### 10. Update Template
```http
PUT /api/v1/ReportTemplates/{id}
Authorization: Bearer {token}
Content-Type: multipart/form-data
```
**Authorization**: Admin only  
**Parameters**:
- `id` (path): Template ID

**Form Data**: Same as Create (File is optional - only provide to replace existing file)

**Returns**: Updated ReportTemplateDto (200), not found (404), or validation error (400)

**Example using curl**:
```bash
curl -X PUT "https://api.example.com/api/v1/ReportTemplates/1" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "Name=Updated Invoice Template" \
  -F "Description=Updated description" \
  -F "TemplateType=0" \
  -F "ReportEngine=FastReport" \
  -F "IsActive=true" \
  -F "IsDefault=false"
```

---

### 11. Set Default Template
```http
PUT /api/v1/ReportTemplates/{id}/set-default
Authorization: Bearer {token}
```
**Authorization**: Admin only  
**Parameters**:
- `id` (path): Template ID

**Returns**: 204 No Content on success, 404 if not found

**Note**: Automatically unsets any other default for the same template type

---

### 12. Toggle Active Status
```http
PUT /api/v1/ReportTemplates/{id}/toggle-active
Authorization: Bearer {token}
```
**Authorization**: Admin only  
**Parameters**:
- `id` (path): Template ID

**Returns**: 204 No Content on success, 404 if not found

---

### 13. Delete Template (Soft Delete)
```http
DELETE /api/v1/ReportTemplates/{id}
Authorization: Bearer {token}
```
**Authorization**: Admin only  
**Parameters**:
- `id` (path): Template ID

**Returns**: 204 No Content on success, 404 if not found

**Note**: Performs soft delete (sets DeletedAt timestamp), data is preserved

---

## Response Models

### ReportTemplateDto
```json
{
  "id": 1,
  "name": "Invoice Template",
  "description": "Standard invoice template",
  "templateType": 0,
  "reportEngine": "FastReport",
  "fileName": "invoice.frx",
  "fileSize": 45678,
  "mimeType": "application/octet-stream",
  "isActive": true,
  "isDefault": true,
  "dataSourceInfo": "{\"fields\":[\"InvoiceNumber\",\"CustomerName\",\"Total\"]}",
  "version": "1.0",
  "tags": "invoice,standard",
  "defaultExportFormat": "PDF",
  "createdAt": "2024-02-04T10:30:00Z",
  "updatedAt": "2024-02-04T10:30:00Z"
}
```

## Error Responses

### 400 Bad Request
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "File": ["File is required"]
  }
}
```

### 401 Unauthorized
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

### 403 Forbidden
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403
}
```

### 404 Not Found
```json
"Report template with ID 123 not found"
```

### 500 Internal Server Error
```json
"An error occurred while creating the report template"
```

## File Upload Requirements

### Allowed Extensions
- `.frx` - FastReport templates
- `.rdlc` - Microsoft Report Definition Language Client-side
- `.rpt` - Crystal Reports templates

### File Size Limit
- Maximum: 50 MB

### MIME Types
- Typically: `application/octet-stream`
- May vary based on file type

## Best Practices

1. **Always check IsActive** before using templates in production
2. **Use default templates** when available for consistency
3. **Tag templates** appropriately for easy searching
4. **Version templates** when making significant changes
5. **Test templates** before setting as default
6. **Download backups** of important templates regularly
7. **Use meaningful names** and descriptions for templates

## Integration Example (C#)

```csharp
public class InvoiceReportService
{
    private readonly HttpClient _httpClient;
    private readonly IReportGenerator _reportGenerator;
    
    public async Task<byte[]> GenerateInvoiceReportAsync(Invoice invoice)
    {
        // 1. Get default invoice template
        var response = await _httpClient.GetAsync(
            "/api/v1/ReportTemplates/default/0");
        
        if (!response.IsSuccessStatusCode)
            throw new Exception("Default invoice template not found");
        
        var template = await response.Content
            .ReadFromJsonAsync<ReportTemplateDto>();
        
        // 2. Download template file
        var fileResponse = await _httpClient.GetAsync(
            $"/api/v1/ReportTemplates/{template.Id}/download");
        
        var templateBytes = await fileResponse.Content.ReadAsByteArrayAsync();
        
        // 3. Save to temp location
        var tempPath = Path.Combine(Path.GetTempPath(), template.FileName);
        await File.WriteAllBytesAsync(tempPath, templateBytes);
        
        // 4. Generate report
        var reportData = MapInvoiceToReportData(invoice);
        var reportBytes = await _reportGenerator.GenerateReportAsync(
            tempPath, 
            reportData, 
            template.DefaultExportFormat);
        
        // 5. Cleanup
        File.Delete(tempPath);
        
        return reportBytes;
    }
}
```

## JavaScript/TypeScript Example

```typescript
// Upload new template
async function uploadReportTemplate(file: File, metadata: any) {
    const formData = new FormData();
    formData.append('File', file);
    formData.append('Name', metadata.name);
    formData.append('Description', metadata.description);
    formData.append('TemplateType', metadata.templateType.toString());
    formData.append('IsActive', metadata.isActive.toString());
    formData.append('IsDefault', metadata.isDefault.toString());
    
    const response = await fetch('/api/v1/ReportTemplates', {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`
        },
        body: formData
    });
    
    if (!response.ok) {
        throw new Error(`Upload failed: ${response.statusText}`);
    }
    
    return await response.json();
}

// Get and download template
async function downloadTemplate(templateId: number) {
    const response = await fetch(
        `/api/v1/ReportTemplates/${templateId}/download`,
        {
            headers: {
                'Authorization': `Bearer ${token}`
            }
        }
    );
    
    if (!response.ok) {
        throw new Error(`Download failed: ${response.statusText}`);
    }
    
    const blob = await response.blob();
    const filename = response.headers
        .get('Content-Disposition')
        ?.split('filename=')[1] || 'template.frx';
    
    // Trigger download in browser
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    window.URL.revokeObjectURL(url);
}
```
