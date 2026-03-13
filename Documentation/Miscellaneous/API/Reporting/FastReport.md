# FastReport Open Source (MIT License)

FastReport Open Source is a free, MIT-licensed reporting engine for .NET applications, compatible with .NET Standard 2.0+ (including .NET 10). It enables developers to create, generate, and export reports from databases or in-memory data sources without runtime fees. Key features include band-oriented report design, C# scripting, support for multiple data sources (SQL, JSON, XML, objects), and export to PDF, HTML, and image formats. It works cross-platform on Windows and Linux (requires libgdiplus) and uses XML report templates (.frx) for easy versioning and integration in desktop, web, or backend services.

## Export Possibilities
FastReport Open Source supports exporting reports to a variety of formats, including:
- PDF (via plugin)
- HTML
- Images: PNG, JPEG, BMP, TIFF
- EMF and other vector graphics formats

## Template Use
Reports are typically designed as **XML templates (.frx)**, which can be:
- Versioned and stored in source control
- Loaded at runtime and filled with dynamic data
- Combined with C# scripting for calculations, conditional formatting, and dynamic content
This makes FastReport Open Source flexible for desktop, web, or backend services while keeping report layouts separate from application logic.

## Installation

```
dotnet add package FastReport.OpenSource
dotnet add package FastReport.OpenSource.Export.Pdf

```

On linux,you probably have to  install **libgdiplus** for grafikk/rendere:
```
sudo apt-get install -y libgdiplus
```

