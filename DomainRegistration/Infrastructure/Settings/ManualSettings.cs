namespace DomainRegistrationLib.Infrastructure.Settings
{
    /// <summary>
    /// Settings for Manual registrar - used for tracking domains registered elsewhere
    /// </summary>
    public class ManualSettings
    {
        public string Notes { get; set; } = "Manual domain management - registered externally";
        public bool AllowOperations { get; set; } = false;
    }
}
