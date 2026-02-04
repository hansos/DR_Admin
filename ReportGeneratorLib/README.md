# ReportGeneratorLib

A library for generating reports such as invoices and customer lists using various report generation providers.

## Overview

ReportGeneratorLib provides a unified interface for generating reports with support for multiple report generation providers. The library follows a factory pattern, making it easy to switch between different providers or add new ones.

## Supported Providers

- **FastReport** - Open-source reporting library with support for multiple export formats

## Features

- Generate reports as byte arrays for in-memory use
- Save reports directly to disk
- Support for multiple output formats (PDF, HTML, Excel, CSV, RTF, XML, Images)
- Template-based report generation
- Configurable output paths and default formats
- Serilog integration for logging

## Installation

Add the ReportGeneratorLib project reference to your application:

```xml
<ItemGroup>
  <ProjectReference Include="..\ReportGeneratorLib\ReportGeneratorLib.csproj" />
</ItemGroup>
```

## Configuration

### appsettings.json

```json
{
  "ReportSettings": {
    "Provider": "FastReport",
    "FastReport": {
      "TemplatesPath": "C:\\Reports\\Templates",
      "OutputPath": "C:\\Reports\\Output",
      "DefaultFormat": "PDF",
      "LicenseKey": null,
      "EnableCache": true,
      "CacheExpirationMinutes": 30
    }
  }
}
```

### Program.cs (Dependency Injection)

```csharp
using ReportGeneratorLib.Factories;
using ReportGeneratorLib.Infrastructure.Settings;
using ReportGeneratorLib.Interfaces;

// Configure settings
builder.Services.Configure<ReportSettings>(
    builder.Configuration.GetSection("ReportSettings"));

// Register factory and service
builder.Services.AddSingleton<ReportGeneratorFactory>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<ReportSettings>>().Value;
    return new ReportGeneratorFactory(settings);
});

builder.Services.AddSingleton<IReportGenerator>(sp =>
{
    var factory = sp.GetRequiredService<ReportGeneratorFactory>();
    return factory.CreateReportGenerator();
});
```

## Usage Examples

### Example 1: Generate Invoice Report as PDF

```csharp
public class InvoiceService
{
    private readonly IReportGenerator _reportGenerator;

    public InvoiceService(IReportGenerator reportGenerator)
    {
        _reportGenerator = reportGenerator;
    }

    public async Task<byte[]> GenerateInvoicePdfAsync(Invoice invoice)
    {
        var reportData = new
        {
            Invoice = invoice,
            InvoiceLines = invoice.Lines,
            Company = invoice.Company,
            Customer = invoice.Customer
        };

        // Generate PDF report
        var pdfBytes = await _reportGenerator.GenerateReportAsync(
            "Invoice.frx", 
            reportData, 
            "PDF");

        return pdfBytes;
    }
}
```

### Example 2: Save Customer List Report to File

```csharp
public class CustomerReportService
{
    private readonly IReportGenerator _reportGenerator;

    public CustomerReportService(IReportGenerator reportGenerator)
    {
        _reportGenerator = reportGenerator;
    }

    public async Task GenerateCustomerListAsync(List<Customer> customers, string outputPath)
    {
        var reportData = new
        {
            Customers = customers,
            GeneratedDate = DateTime.Now,
            TotalCount = customers.Count
        };

        // Save as Excel file
        await _reportGenerator.SaveReportAsync(
            "CustomerList.frx",
            reportData,
            outputPath,
            "XLSX");
    }
}
```

### Example 3: Generate Multiple Formats

```csharp
public class ReportExportService
{
    private readonly IReportGenerator _reportGenerator;

    public ReportExportService(IReportGenerator reportGenerator)
    {
        _reportGenerator = reportGenerator;
    }

    public async Task<Dictionary<string, byte[]>> GenerateMultipleFormatsAsync(object data)
    {
        var results = new Dictionary<string, byte[]>();

        // Generate PDF
        results["PDF"] = await _reportGenerator.GenerateReportAsync(
            "SalesReport.frx", data, "PDF");

        // Generate Excel
        results["Excel"] = await _reportGenerator.GenerateReportAsync(
            "SalesReport.frx", data, "XLSX");

        // Generate HTML
        results["HTML"] = await _reportGenerator.GenerateReportAsync(
            "SalesReport.frx", data, "HTML");

        return results;
    }
}
```

## FastReport Provider

### Supported Export Formats

- **PDF** - Portable Document Format
- **HTML** - Web page format
- **XLSX/EXCEL** - Microsoft Excel format
- **CSV** - Comma-Separated Values
- **RTF** - Rich Text Format
- **XML** - Extensible Markup Language
- **PNG/IMAGE** - PNG image format
- **JPEG/JPG** - JPEG image format

### Template Structure

FastReport templates (.frx files) should be designed using FastReport Designer. The library automatically registers the data object with the name "Data", which can be accessed in your templates.

### Configuration Options

- **TemplatesPath**: Directory where report templates (.frx files) are stored
- **OutputPath**: Directory for temporary files during report generation
- **DefaultFormat**: Default export format (PDF, XLSX, HTML, etc.)
- **LicenseKey**: Optional FastReport license key (for commercial version)
- **EnableCache**: Enable template caching for better performance
- **CacheExpirationMinutes**: Cache expiration time in minutes

## Interface: IReportGenerator

```csharp
public interface IReportGenerator
{
    Task<byte[]> GenerateReportAsync(string reportTemplate, object data);
    Task<byte[]> GenerateReportAsync(string reportTemplate, object data, string outputFormat);
    Task SaveReportAsync(string reportTemplate, object data, string outputPath);
    Task SaveReportAsync(string reportTemplate, object data, string outputPath, string outputFormat);
}
```

### Methods

- **GenerateReportAsync(template, data)**: Generate report with default format, returns byte array
- **GenerateReportAsync(template, data, format)**: Generate report with specified format, returns byte array
- **SaveReportAsync(template, data, path)**: Save report to file with default format
- **SaveReportAsync(template, data, path, format)**: Save report to file with specified format

## Error Handling

The library throws exceptions for common error scenarios:

- `FileNotFoundException`: When report template is not found
- `InvalidOperationException`: When report preparation fails or settings are not configured
- `NotSupportedException`: When an unsupported export format is specified

All errors are logged using Serilog for troubleshooting.

## Adding New Providers

To add a new report provider:

1. Create a settings class in `Infrastructure/Settings/`
2. Add the new provider to `ReportSettings`
3. Implement `IReportGenerator` in `Implementations/`
4. Update `ReportGeneratorFactory` to support the new provider

## License

This library uses FastReport.OpenSource which is licensed under the MIT license. For commercial FastReport features, you may need to purchase a license.

## Dependencies

- .NET 10.0
- FastReport.Core 2025.1.0-demo
- Serilog 4.3.0
- Serilog.AspNetCore 10.0.0
- Serilog.Enrichers.Environment 3.0.1
- Serilog.Enrichers.Process 3.0.0
- Serilog.Enrichers.Thread 4.0.0

## Notes

- The library uses FastReport.Core (demo version) which includes watermarks. For production use, you'll need to purchase a FastReport license and configure the `LicenseKey` in settings.
- Additional export formats (Excel, CSV, etc.) are loaded dynamically via reflection from FastReport.Compat package.
- Data objects are automatically converted to DataTables for compatibility with FastReport's data binding system.

