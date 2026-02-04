# Usage Examples

This file contains code examples for using ReportGeneratorLib. These are for reference only and should be adapted to your specific application.

## Example 1: Configure services in ASP.NET Core

```csharp
using ReportGeneratorLib.Factories;
using ReportGeneratorLib.Infrastructure.Settings;
using ReportGeneratorLib.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Configure ReportSettings from appsettings.json
    services.Configure<ReportSettings>(
        configuration.GetSection("ReportSettings"));

    // Register factory and service
    services.AddSingleton<ReportGeneratorFactory>(sp =>
    {
        var settings = sp.GetRequiredService<IOptions<ReportSettings>>().Value;
        return new ReportGeneratorFactory(settings);
    });

    services.AddSingleton<IReportGenerator>(sp =>
    {
        var factory = sp.GetRequiredService<ReportGeneratorFactory>();
        return factory.CreateReportGenerator();
    });
}
```

## Example 2: Generate Invoice PDF

```csharp
public class InvoiceService
{
    private readonly IReportGenerator _reportGenerator;

    public InvoiceService(IReportGenerator reportGenerator)
    {
        _reportGenerator = reportGenerator;
    }

    public async Task<byte[]> GenerateInvoicePdfAsync(int invoiceId)
    {
        // Prepare invoice data
        var invoice = new
        {
            InvoiceNumber = "INV-2024-001",
            InvoiceDate = DateTime.Now,
            DueDate = DateTime.Now.AddDays(30),
            CustomerName = "Acme Corporation",
            CustomerAddress = "123 Business St, City, ST 12345",
            Items = new[]
            {
                new { Description = "Web Development", Quantity = 40, UnitPrice = 150.00m, Total = 6000.00m },
                new { Description = "Hosting Services", Quantity = 1, UnitPrice = 99.00m, Total = 99.00m }
            },
            Subtotal = 6099.00m,
            Tax = 609.90m,
            Total = 6708.90m
        };

        // Generate PDF
        var pdfBytes = await _reportGenerator.GenerateReportAsync(
            "Invoice.frx",
            invoice,
            "PDF");

        return pdfBytes;
    }

    public async Task SaveInvoiceToFileAsync(int invoiceId, string filePath)
    {
        var invoice = GetInvoiceData(invoiceId);

        await _reportGenerator.SaveReportAsync(
            "Invoice.frx",
            invoice,
            filePath,
            "PDF");
    }
}
```

## Example 3: Generate Customer List Report

```csharp
public class CustomerReportService
{
    private readonly IReportGenerator _reportGenerator;

    public CustomerReportService(IReportGenerator reportGenerator)
    {
        _reportGenerator = reportGenerator;
    }

    public async Task<byte[]> GenerateCustomerListExcelAsync()
    {
        var customers = new List<object>
        {
            new { Id = 1, Name = "Acme Corporation", Email = "contact@acme.com", Phone = "555-1234", Active = true },
            new { Id = 2, Name = "Globex Inc", Email = "info@globex.com", Phone = "555-5678", Active = true },
            new { Id = 3, Name = "Initech", Email = "sales@initech.com", Phone = "555-9012", Active = false }
        };

        var excelBytes = await _reportGenerator.GenerateReportAsync(
            "CustomerList.frx",
            customers,
            "XLSX");

        return excelBytes;
    }

    public async Task ExportCustomersToMultipleFormatsAsync(string outputDirectory)
    {
        var customers = GetCustomers();

        // Generate PDF
        await _reportGenerator.SaveReportAsync(
            "CustomerList.frx",
            customers,
            Path.Combine(outputDirectory, "customers.pdf"),
            "PDF");

        // Generate Excel
        await _reportGenerator.SaveReportAsync(
            "CustomerList.frx",
            customers,
            Path.Combine(outputDirectory, "customers.xlsx"),
            "XLSX");

        // Generate CSV
        await _reportGenerator.SaveReportAsync(
            "CustomerList.frx",
            customers,
            Path.Combine(outputDirectory, "customers.csv"),
            "CSV");
    }
}
```

## Example 4: Manual creation without Dependency Injection

```csharp
public async Task GenerateReportManuallyAsync()
{
    // Create settings
    var settings = new ReportSettings
    {
        Provider = "FastReport",
        FastReport = new FastReport
        {
            TemplatesPath = @"C:\Reports\Templates",
            OutputPath = @"C:\Reports\Output",
            DefaultFormat = "PDF",
            EnableCache = true,
            CacheExpirationMinutes = 30
        }
    };

    // Create factory and generator
    var factory = new ReportGeneratorFactory(settings);
    var generator = factory.CreateReportGenerator();

    // Generate report
    var data = new
    {
        Title = "Sales Report",
        Date = DateTime.Now,
        Items = new[]
        {
            new { Product = "Widget A", Sales = 1250.00m },
            new { Product = "Widget B", Sales = 3400.00m }
        }
    };

    var pdfBytes = await generator.GenerateReportAsync(
        "SalesReport.frx",
        data,
        "PDF");

    // Save to file
    await File.WriteAllBytesAsync(@"C:\Reports\Output\sales-report.pdf", pdfBytes);
}
```

## Example 5: Using with ASP.NET Core Controller

```csharp
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportGenerator _reportGenerator;

    public ReportsController(IReportGenerator reportGenerator)
    {
        _reportGenerator = reportGenerator;
    }

    [HttpGet("invoice/{id}")]
    public async Task<IActionResult> GetInvoicePdf(int id)
    {
        try
        {
            var invoice = GetInvoiceById(id);

            var pdfBytes = await _reportGenerator.GenerateReportAsync(
                "Invoice.frx",
                invoice,
                "PDF");

            return File(pdfBytes, "application/pdf", $"Invoice-{id}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("customers/export")]
    public async Task<IActionResult> ExportCustomers([FromQuery] string format = "PDF")
    {
        try
        {
            var customers = GetAllCustomers();

            var bytes = await _reportGenerator.GenerateReportAsync(
                "CustomerList.frx",
                customers,
                format.ToUpper());

            var contentType = format.ToUpper() switch
            {
                "PDF" => "application/pdf",
                "XLSX" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "CSV" => "text/csv",
                "HTML" => "text/html",
                _ => "application/octet-stream"
            };

            return File(bytes, contentType, $"customers.{format.ToLower()}");
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private object GetInvoiceById(int id)
    {
        // Replace with actual data retrieval
        return new { InvoiceNumber = $"INV-{id}", /* ... */ };
    }

    private List<object> GetAllCustomers()
    {
        // Replace with actual data retrieval
        return new List<object>();
    }
}
```

## Example 6: appsettings.json Configuration

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
