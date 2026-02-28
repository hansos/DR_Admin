namespace ReportGeneratorLib.Infrastructure.Settings
{
    public class QuestPdf
    {
        public string OutputPath { get; set; } = string.Empty;
        public string DefaultFormat { get; set; } = "PDF";
        public string LicenseType { get; set; } = "Community";
    }
}
