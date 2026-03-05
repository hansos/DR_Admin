using EmailSenderLib.Infrastructure.Settings;

namespace ISPAdmin.Infrastructure.Settings;

public class AppSettings
{
    public string DefaultConnection { get; set; } = string.Empty;
    public string FrontendBaseUrl { get; set; } = "https://localhost:5001";
    public string EmailConfirmationPath { get; set; } = "/confirm-email";
    public string PasswordResetPath { get; set; } = "/reset-password";
    public string DefaultFrontendSiteCode { get; set; } = "reseller";
    public List<FrontendSiteSettings> FrontendSites { get; set; } = [];
    public DbSettings DbSettings { get; set; } = new();
    public EmailSettings? MailSettings { get; set; }
}

public class FrontendSiteSettings
{
    public string Code { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string EmailConfirmationPath { get; set; } = "/confirm-email";
    public string PasswordResetPath { get; set; } = "/reset-password";
}
