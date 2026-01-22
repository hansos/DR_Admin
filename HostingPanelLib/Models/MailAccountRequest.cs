namespace HostingPanelLib.Models
{
    public class MailAccountRequest
    {
        public string EmailAddress { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? QuotaMB { get; set; }
        public string? DisplayName { get; set; }
        public bool? EnableImap { get; set; }
        public bool? EnablePop3 { get; set; }
        public bool? EnableSmtp { get; set; }
        public List<string>? ForwardTo { get; set; }
        public bool? EnableAutoresponder { get; set; }
        public string? AutoresponderMessage { get; set; }
        public Dictionary<string, string>? AdditionalSettings { get; set; }
    }
}
