using EmailSenderLib.Infrastructure.Settings;

namespace ISPAdmin.Infrastructure.Settings;

public class AppSettings
{
    public string DefaultConnection { get; set; } = string.Empty;
    public DbSettings DbSettings { get; set; } = new();
    public EmailSettings? MailSettings { get; set; }
}
