using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportGeneratorLib.Helpers;
using ReportGeneratorLib.Infrastructure.Enums;
using ReportGeneratorLib.Interfaces;
using ReportGeneratorLib.Models;
using Serilog;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ReportGeneratorLib.Implementations
{
    public class QuestPdfReportGenerator : IReportGenerator
    {
        private readonly string _outputPath;
        private readonly string _defaultFormat;

        public QuestPdfReportGenerator(
            string outputPath,
            string defaultFormat = "PDF",
            string licenseType = "Community")
        {
            _outputPath = outputPath;
            _defaultFormat = defaultFormat;

            ConfigureLicense(licenseType);

            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }
        }

        public async Task<byte[]> GenerateReportAsync(ReportType type, object data, OutputFormat? outputFormat = OutputFormat.Pdf)
        {
            ValidateOutputFormat(outputFormat);
            return await GenerateReportByTypeAsync(type, data);
        }

        public async Task SaveReportAsync(ReportType type, object data, string outputPath, OutputFormat? outputFormat = OutputFormat.Pdf)
        {
            ValidateOutputFormat(outputFormat);

            var bytes = await GenerateReportByTypeAsync(type, data);
            var outputDirectory = Path.GetDirectoryName(outputPath);

            if (!string.IsNullOrWhiteSpace(outputDirectory) && !Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            await File.WriteAllBytesAsync(outputPath, bytes);
        }

        private static async Task<byte[]> GenerateReportByTypeAsync(ReportType type, object data)
        {
            return type switch
            {
                ReportType.Offer => await GenerateOfferReportAsync(data),
                ReportType.Order => await GenerateOrderReportAsync(data),
                _ => throw new NotSupportedException($"Report type '{type}' is not supported by QuestPDF provider.")
            };
        }

        private static Task<byte[]> GenerateOrderReportAsync(object data)
        {
            return Task.Run(() => GeneratePdfBytes("Order Report", data));
        }

        private static Task<byte[]> GenerateOfferReportAsync(object data)
        {
            var offer = DeserializeOfferData(data);
            return Task.Run(() => GenerateOfferPdfBytes.Create(offer));
        }

        [Obsolete("Use GenerateReportAsync(ReportType, object, OutputFormat?);")]
        public async Task<byte[]> GenerateReportAsync(string reportTemplate, object data)
        {
            return await GenerateReportAsync(reportTemplate, data, _defaultFormat);
        }

        [Obsolete("Use GenerateReportAsync(ReportType, object, OutputFormat?);")]
        public async Task<byte[]> GenerateReportAsync(string reportTemplate, object data, string outputFormat)
        {
            ValidateOutputFormat(outputFormat);

            var reportName = string.IsNullOrWhiteSpace(reportTemplate)
                ? "Report"
                : Path.GetFileNameWithoutExtension(reportTemplate);

            return await Task.Run(() => GeneratePdfBytes(reportName, data));
        }

        [Obsolete("Use GenerateReportAsync(ReportType, object, string, OutputFormat?);")]
        public async Task SaveReportAsync(string reportTemplate, object data, string outputPath)
        {
            await SaveReportAsync(reportTemplate, data, outputPath, _defaultFormat);
        }

        [Obsolete("Use GenerateReportAsync(ReportType, object, string, OutputFormat?);")]
        public async Task SaveReportAsync(string reportTemplate, object data, string outputPath, string outputFormat)
        {
            ValidateOutputFormat(outputFormat);

            var bytes = await GenerateReportAsync(reportTemplate, data, outputFormat);
            var outputDirectory = Path.GetDirectoryName(outputPath);

            if (!string.IsNullOrWhiteSpace(outputDirectory) && !Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            await File.WriteAllBytesAsync(outputPath, bytes);
        }

        private static byte[] GeneratePdfBytes(string reportName, object data)
        {
            var jsonData = SerializeData(data);

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header()
                        .Text(reportName)
                        .SemiBold()
                        .FontSize(18);

                    page.Content().Column(column =>
                    {
                        column.Spacing(8);

                        column.Item()
                            .Text("Report Data")
                            .Bold()
                            .FontSize(12);

                        column.Item()
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(10)
                            .Text(jsonData)
                            .FontSize(9);
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Generated at ");
                            text.Span(DateTime.UtcNow.ToString("u")).SemiBold();
                        });
                });
            }).GeneratePdf();
        }

        private static OfferDocumentDto DeserializeOfferData(object data)
        {
            if (data is OfferDocumentDto dto)
            {
                return dto;
            }

            if (data is JsonElement jsonElement)
            {
                var parsedFromElement = jsonElement.Deserialize<OfferDocumentDto>();
                if (parsedFromElement != null)
                {
                    return parsedFromElement;
                }
            }

            var serialized = JsonSerializer.Serialize(data);
            var parsed = JsonSerializer.Deserialize<OfferDocumentDto>(serialized);
            if (parsed == null)
            {
                throw new InvalidOperationException("Unable to deserialize offer report data to OfferDocumentDto.");
            }

            return parsed;
        }

        private static string SerializeData(object data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            return JsonSerializer.Serialize(data, options);
        }

        private static void ValidateOutputFormat(OutputFormat? outputFormat)
        {
            if (outputFormat != OutputFormat.Pdf)
            {
                throw new NotSupportedException("QuestPDF provider currently supports only PDF output format.");
            }
        }

        private static void ValidateOutputFormat(string outputFormat)
        {
            if (!string.Equals(outputFormat, "PDF", StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException("QuestPDF provider currently supports only PDF output format.");
            }
        }

        private static void ConfigureLicense(string licenseType)
        {
            try
            {
                QuestPDF.Settings.License = Enum.TryParse<LicenseType>(licenseType, true, out var parsed)
                    ? parsed
                    : LicenseType.Community;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Invalid QuestPDF license type '{LicenseType}', defaulting to Community", licenseType);
                QuestPDF.Settings.License = LicenseType.Community;
            }
        }
    }
}
