using ReportGeneratorLib.Implementations;
using ReportGeneratorLib.Infrastructure.Settings;
using ReportGeneratorLib.Interfaces;
using System;

namespace ReportGeneratorLib.Factories
{
    public class ReportGeneratorFactory
    {
        private readonly ReportSettings _reportSettings;

        public ReportGeneratorFactory(ReportSettings reportSettings)
        {
            _reportSettings = reportSettings ?? throw new ArgumentNullException(nameof(reportSettings));
        }

        public IReportGenerator CreateReportGenerator()
        {
            return _reportSettings.Provider.ToLower() switch
            {
                "fastreport" => _reportSettings.FastReport is not null
                    ? new FastReportGenerator(
                        _reportSettings.FastReport.TemplatesPath,
                        _reportSettings.FastReport.OutputPath,
                        _reportSettings.FastReport.DefaultFormat,
                        _reportSettings.FastReport.LicenseKey,
                        _reportSettings.FastReport.EnableCache,
                        _reportSettings.FastReport.CacheExpirationMinutes
                    )
                    : throw new InvalidOperationException("FastReport settings are not configured"),

                _ => throw new NotSupportedException($"Report provider '{_reportSettings.Provider}' is not supported")
            };
        }
    }
}
