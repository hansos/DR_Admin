namespace DomainRegistrationLib.Infrastructure.Settings
{
    public class AwsSettings
    {
        public string AccessKeyId { get; set; } = string.Empty;
        public string SecretAccessKey { get; set; } = string.Empty;
        public string Region { get; set; } = "us-east-1";
        public string Route53HostedZoneId { get; set; } = string.Empty;
    }
}
