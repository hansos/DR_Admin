using System;
using System.Collections.Generic;
using System.Text;

namespace ReportGeneratorLib.Interfaces
{
    public interface IReportGenerator
    {
        Task<byte[]> GenerateReportAsync(string reportTemplate, object data);
        Task<byte[]> GenerateReportAsync(string reportTemplate, object data, string outputFormat);
        Task SaveReportAsync(string reportTemplate, object data, string outputPath);
        Task SaveReportAsync(string reportTemplate, object data, string outputPath, string outputFormat);
    }
}
