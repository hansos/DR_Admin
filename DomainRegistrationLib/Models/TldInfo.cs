namespace DomainRegistrationLib.Models
{
    public class TldInfo
    {
        /// <summary>
        /// The top-level domain name (e.g., "com", "net", "org")
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Registration price for the TLD
        /// </summary>
        public decimal? RegistrationPrice { get; set; }

        /// <summary>
        /// Renewal price for the TLD
        /// </summary>
        public decimal? RenewalPrice { get; set; }

        /// <summary>
        /// Transfer price for the TLD
        /// </summary>
        public decimal? TransferPrice { get; set; }

        /// <summary>
        /// Currency code for the prices (e.g., "USD", "EUR")
        /// </summary>
        public string? Currency { get; set; }

        /// <summary>
        /// Minimum registration period in years
        /// </summary>
        public int? MinRegistrationYears { get; set; }

        /// <summary>
        /// Maximum registration period in years
        /// </summary>
        public int? MaxRegistrationYears { get; set; }

        /// <summary>
        /// Whether the TLD supports privacy/WHOIS protection
        /// </summary>
        public bool? SupportsPrivacy { get; set; }

        /// <summary>
        /// Whether the TLD supports DNSSEC
        /// </summary>
        public bool? SupportsDnssec { get; set; }

        /// <summary>
        /// Whether the TLD is a generic TLD (gTLD)
        /// </summary>
        public bool? IsGeneric { get; set; }

        /// <summary>
        /// Whether the TLD is a country-code TLD (ccTLD)
        /// </summary>
        public bool? IsCountryCode { get; set; }

        /// <summary>
        /// Type/category of the TLD (e.g., "generic", "country", "sponsored")
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Whether registration is currently available for this TLD
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Additional notes or restrictions for the TLD
        /// </summary>
        public string? Notes { get; set; }
    }
}
