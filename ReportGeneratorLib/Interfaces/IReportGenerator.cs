using Microsoft.Extensions.Configuration.UserSecrets;
using ReportGeneratorLib.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportGeneratorLib.Interfaces
{
    public interface IReportGenerator
    {
        Task<byte[]> GenerateReportAsync(ReportType type, object data, OutputFormat? outputFormat=OutputFormat.Pdf);

        Task SaveReportAsync(ReportType type, object data, string outputPath, OutputFormat? outputFormat = OutputFormat.Pdf);

        [Obsolete("Use GenerateReportAsync(ReportType, object, string, OutputFormat?);")]
        Task SaveReportAsync(string reportTemplate, object data, string outputPath);

        [Obsolete("Use GenerateReportAsync(ReportType, object, string, OutputFormat?);")]
        Task SaveReportAsync(string reportTemplate, object data, string outputPath, string outputFormat);


        [Obsolete("Use GenerateReportAsync(ReportType, object, OutputFormat?);")]
        Task<byte[]> GenerateReportAsync(string reportTemplate, object data);

        [Obsolete("Use GenerateReportAsync(ReportType, object, OutputFormat?);")]
        Task<byte[]> GenerateReportAsync(string reportTemplate, object data, string outputFormat);

    }
}
