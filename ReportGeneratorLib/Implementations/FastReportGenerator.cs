using ReportGeneratorLib.Interfaces;
using Serilog;
using FastReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ReportGeneratorLib.Implementations
{
    public class FastReportGenerator : IReportGenerator
    {
        private readonly string _templatesPath;
        private readonly string _outputPath;
        private readonly string _defaultFormat;
        private readonly string? _licenseKey;
        private readonly bool _enableCache;
        private readonly int _cacheExpirationMinutes;

        public FastReportGenerator(
            string templatesPath,
            string outputPath,
            string defaultFormat = "PDF",
            string? licenseKey = null,
            bool enableCache = true,
            int cacheExpirationMinutes = 30)
        {
            _templatesPath = templatesPath;
            _outputPath = outputPath;
            _defaultFormat = defaultFormat;
            _licenseKey = licenseKey;
            _enableCache = enableCache;
            _cacheExpirationMinutes = cacheExpirationMinutes;

            // Register FastReport license if provided
            if (!string.IsNullOrEmpty(_licenseKey))
            {
                try
                {
                    Log.Information("FastReport license key configured");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error registering FastReport license");
                }
            }

            // Ensure output directory exists
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }
        }

        public async Task<byte[]> GenerateReportAsync(string reportTemplate, object data)
        {
            return await GenerateReportAsync(reportTemplate, data, _defaultFormat);
        }

        public async Task<byte[]> GenerateReportAsync(string reportTemplate, object data, string outputFormat)
        {
            try
            {
                Log.Information("Generating report: {ReportTemplate} in format: {OutputFormat}", reportTemplate, outputFormat);

                var templatePath = Path.Combine(_templatesPath, reportTemplate);
                if (!File.Exists(templatePath))
                {
                    throw new FileNotFoundException($"Report template not found: {templatePath}");
                }

                // Create a temporary file for the output
                var tempOutputFile = Path.Combine(_outputPath, $"{Guid.NewGuid()}.{outputFormat.ToLower()}");

                using (var report = new Report())
                {
                    // Load the report template
                    report.Load(templatePath);

                    // Register data for the report
                    if (data != null)
                    {
                        RegisterDataSource(report, data);
                    }

                    // Prepare the report
                    if (!report.Prepare())
                    {
                        throw new InvalidOperationException("Report preparation failed");
                    }

                    // Export based on format
                    var exportResult = await ExportReportAsync(report, tempOutputFile, outputFormat);

                    // Read the generated file
                    var bytes = await File.ReadAllBytesAsync(exportResult);

                    // Clean up temporary file
                    if (File.Exists(exportResult))
                    {
                        File.Delete(exportResult);
                    }

                    Log.Information("Report generated successfully: {ReportTemplate}", reportTemplate);
                    return bytes;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating report: {ReportTemplate}", reportTemplate);
                throw;
            }
        }

        public async Task SaveReportAsync(string reportTemplate, object data, string outputPath)
        {
            await SaveReportAsync(reportTemplate, data, outputPath, _defaultFormat);
        }

        public async Task SaveReportAsync(string reportTemplate, object data, string outputPath, string outputFormat)
        {
            try
            {
                Log.Information("Saving report: {ReportTemplate} to {OutputPath} in format: {OutputFormat}", 
                    reportTemplate, outputPath, outputFormat);

                var templatePath = Path.Combine(_templatesPath, reportTemplate);
                if (!File.Exists(templatePath))
                {
                    throw new FileNotFoundException($"Report template not found: {templatePath}");
                }

                // Ensure output directory exists
                var outputDirectory = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                using (var report = new Report())
                {
                    // Load the report template
                    report.Load(templatePath);

                    // Register data for the report
                    if (data != null)
                    {
                        RegisterDataSource(report, data);
                    }

                    // Prepare the report
                    if (!report.Prepare())
                    {
                        throw new InvalidOperationException("Report preparation failed");
                    }

                    // Export to specified path
                    await ExportReportAsync(report, outputPath, outputFormat);
                }

                Log.Information("Report saved successfully: {ReportTemplate} to {OutputPath}", reportTemplate, outputPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error saving report: {ReportTemplate} to {OutputPath}", reportTemplate, outputPath);
                throw;
            }
        }

        private void RegisterDataSource(Report report, object data)
        {
            // Handle different data types
            if (data is DataSet dataSet)
            {
                report.RegisterData(dataSet, "Data");
            }
            else if (data is DataTable dt)
            {
                var ds = new DataSet();
                ds.Tables.Add(dt);
                report.RegisterData(ds, "Data");
            }
            else
            {
                // For objects and collections, we need to use reflection to call the generic method
                // or convert to DataTable
                var convertedTable = ConvertToDataTable(data);
                var ds = new DataSet();
                ds.Tables.Add(convertedTable);
                report.RegisterData(ds, "Data");
            }
        }

        private DataTable ConvertToDataTable(object data)
        {
            var dataTable = new DataTable("Data");

            if (data is System.Collections.IEnumerable enumerable)
            {
                bool headersAdded = false;
                foreach (var item in enumerable)
                {
                    if (item == null) continue;

                    if (!headersAdded)
                    {
                        // Add columns based on first item properties
                        var props = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        foreach (var prop in props)
                        {
                            dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                        }
                        headersAdded = true;
                    }

                    // Add row with values
                    var row = dataTable.NewRow();
                    var properties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in properties)
                    {
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                    }
                    dataTable.Rows.Add(row);
                }
            }
            else
            {
                // Single object - add as single row
                var props = data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in props)
                {
                    dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }

                var row = dataTable.NewRow();
                foreach (var prop in props)
                {
                    row[prop.Name] = prop.GetValue(data) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        private async Task<string> ExportReportAsync(Report report, string outputPath, string format)
        {
            return await Task.Run(() =>
            {
                // Use reflection to find and create the appropriate export type
                var exportTypeName = format.ToUpper() switch
                {
                    "PDF" => "FastReport.Export.Pdf.PDFExport, FastReport.Compat",
                    "HTML" => "FastReport.Export.Html.HTMLExport, FastReport.Compat",
                    "XLSX" or "EXCEL" => "FastReport.Export.OoXML.Excel2007Export, FastReport.Compat",
                    "CSV" => "FastReport.Export.Csv.CSVExport, FastReport.Compat",
                    "RTF" => "FastReport.Export.RtfExport, FastReport.Compat",
                    "XML" => "FastReport.Export.Xml.XMLExport, FastReport.Compat",
                    "IMAGE" or "PNG" => "FastReport.Export.Image.ImageExport, FastReport.Compat",
                    "JPEG" or "JPG" => "FastReport.Export.Image.ImageExport, FastReport.Compat",
                    _ => null
                };

                if (exportTypeName == null)
                {
                    throw new NotSupportedException($"Export format '{format}' is not supported.");
                }

                var exportType = Type.GetType(exportTypeName);
                if (exportType == null)
                {
                    throw new NotSupportedException($"Export format '{format}' is not available. " +
                        "You may need to install additional FastReport export packages.");
                }

                using (dynamic export = Activator.CreateInstance(exportType)!)
                {
                    // Set format-specific properties
                    if (format.ToUpper() == "HTML")
                    {
                        try
                        {
                            export.SinglePage = true;
                            export.Navigator = false;
                        }
                        catch
                        {
                            // Ignore if properties don't exist
                        }
                    }
                    else if (format.ToUpper() == "PNG" || format.ToUpper() == "IMAGE")
                    {
                        try
                        {
                            // ImageExportFormat.Png = 0
                            var imageExportFormatType = Type.GetType("FastReport.Export.Image.ImageExportFormat, FastReport.Compat");
                            if (imageExportFormatType != null)
                            {
                                export.ImageFormat = Enum.Parse(imageExportFormatType, "Png");
                            }
                        }
                        catch
                        {
                            // Ignore if property doesn't exist
                        }
                    }
                    else if (format.ToUpper() == "JPEG" || format.ToUpper() == "JPG")
                    {
                        try
                        {
                            // ImageExportFormat.Jpeg = 1
                            var imageExportFormatType = Type.GetType("FastReport.Export.Image.ImageExportFormat, FastReport.Compat");
                            if (imageExportFormatType != null)
                            {
                                export.ImageFormat = Enum.Parse(imageExportFormatType, "Jpeg");
                            }
                        }
                        catch
                        {
                            // Ignore if property doesn't exist
                        }
                    }

                    report.Export(export, outputPath);
                }

                return outputPath;
            });
        }
    }
}
