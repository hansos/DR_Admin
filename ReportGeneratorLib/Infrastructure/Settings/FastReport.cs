namespace ReportGeneratorLib.Infrastructure.Settings
{
    public class FastReport
    {
        public string TemplatesPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public string DefaultFormat { get; set; } = "PDF";
        public string? LicenseKey { get; set; }
        public bool EnableCache { get; set; } = true;
        public int CacheExpirationMinutes { get; set; } = 30;
    }
}
