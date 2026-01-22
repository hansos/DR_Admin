namespace EmailSenderLib.Infrastructure.Settings
{
    public class AmazonSes
    {
        public string AccessKeyId { get; set; } = string.Empty;
        public string SecretAccessKey { get; set; } = string.Empty;
        public string Region { get; set; } = "us-east-1";
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
    }
}
