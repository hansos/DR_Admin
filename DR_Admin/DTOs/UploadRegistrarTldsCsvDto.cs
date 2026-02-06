using Microsoft.AspNetCore.Http;

namespace ISPAdmin.DTOs
{
    /// <summary>
    /// Data transfer object for uploading a CSV file with TLDs for a registrar
    /// </summary>
    public class UploadRegistrarTldsCsvDto
    {
        /// <summary>
        /// Gets or sets the CSV file to upload
        /// Expected CSV format: Tld, Description
        /// Example:
        /// ac,
        /// academy,
        /// airforce, US Airforce only
        /// </summary>
        public IFormFile? File { get; set; }

        /// <summary>
        /// Gets or sets the default registration cost for imported TLDs
        /// </summary>
        public decimal? DefaultRegistrationCost { get; set; }

        /// <summary>
        /// Gets or sets the default registration price for imported TLDs
        /// </summary>
        public decimal? DefaultRegistrationPrice { get; set; }

        /// <summary>
        /// Gets or sets the default renewal cost for imported TLDs
        /// </summary>
        public decimal? DefaultRenewalCost { get; set; }

        /// <summary>
        /// Gets or sets the default renewal price for imported TLDs
        /// </summary>
        public decimal? DefaultRenewalPrice { get; set; }

        /// <summary>
        /// Gets or sets the default transfer cost for imported TLDs
        /// </summary>
        public decimal? DefaultTransferCost { get; set; }

        /// <summary>
        /// Gets or sets the default transfer price for imported TLDs
        /// </summary>
        public decimal? DefaultTransferPrice { get; set; }

        /// <summary>
        /// Gets or sets whether imported TLDs should be marked as available for purchase
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to activate TLDs that don't exist in the Tlds table
        /// </summary>
        public bool ActivateNewTlds { get; set; } = false;

        /// <summary>
        /// Gets or sets the currency for pricing (default: USD)
        /// </summary>
        public string Currency { get; set; } = "USD";
    }
}
