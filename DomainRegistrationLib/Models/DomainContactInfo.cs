namespace DomainRegistrationLib.Models
{
    /// <summary>
    /// Contact information for a domain registration.
    /// This model is designed to be compatible with domain contact storage requirements.
    /// </summary>
    public class DomainContactInfo
    {
        /// <summary>
        /// Gets or sets the type of contact (Registrant, Admin, Technical, Billing)
        /// </summary>
        public string ContactType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name of the contact person
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the contact person
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the organization/company name
        /// </summary>
        public string? Organization { get; set; }

        /// <summary>
        /// Gets or sets the email address of the contact person
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the phone number of the contact person
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the fax number of the contact person
        /// </summary>
        public string? Fax { get; set; }

        /// <summary>
        /// Gets or sets the first line of the address
        /// </summary>
        public string Address1 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the second line of the address
        /// </summary>
        public string? Address2 { get; set; }

        /// <summary>
        /// Gets or sets the city
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the state/province/region
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Gets or sets the postal/ZIP code
        /// </summary>
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the country code (ISO 3166-1 alpha-2)
        /// </summary>
        public string CountryCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether this contact is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets additional notes about the contact
        /// </summary>
        public string? Notes { get; set; }
    }
}
